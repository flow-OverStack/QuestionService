using System.Net;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Comparers;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services;

public class ViewService(
    ICacheProvider cache,
    IBaseRepository<Question> questionRepository,
    IBaseRepository<View> viewRepository,
    IEntityProvider<UserDto> userProvider,
    IOptions<BusinessRules> businessRules)
    : IViewService, IViewDatabaseService
{
    private const char ViewKeySeparator = ',';
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<SyncedViewsDto>> SyncViewsToDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        // Removing invalid keys

        var allViewKeys =
            (await cache.SetStringMembersAsync(CacheKeyHelper.GetViewQuestionsKey(), cancellationToken)).ToArray();
        var validViewKeys = allViewKeys.Where(IsValidKey).ToArray();

        // Removing invalid values

        var allViewValues =
            (await cache.SetsStringMembersAsync(validViewKeys, cancellationToken)).ToArray();
        var validViewValues = allViewValues.Select(x =>
            new KeyValuePair<string, IEnumerable<string>>(x.Key, x.Value.Where(IsValidValue)));

        var spamFilteredViews = validViewValues.FilterByMaxValueOccurrences(ViewParsingHelpers.GetKeyFromValue,
            _businessRules.UserViewSpamThreshold);

        var parsedViews = spamFilteredViews
            .SelectManyFromGroupedValues(ViewParsingHelpers.ParseQuestionIdFromKey,
                ViewParsingHelpers.ParseViewFromValue).ToArray();

        if (parsedViews.Length == 0) return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(0));

        // Filtering views to have real user and question ids and to be unique

        var predicate = PredicateBuilder.New<View>();
        predicate = parsedViews.Aggregate(predicate,
            (current, local) =>
                current.Or(UniqueViewComparer.ViewEquals(local)));

        var existingViews = await viewRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToHashSetAsync(new UniqueViewComparer(), cancellationToken);
        var existingQuestionIds = await questionRepository.GetAll()
            .Where(x => parsedViews.Select(y => y.QuestionId).Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);
        var existingUserIds = (await userProvider.GetByIdsAsync(
                parsedViews.Where(x => x.UserId != null).Select(x => (long)x.UserId!),
                cancellationToken))
            .Select(x => x.Id)
            .ToHashSet();

        // Hash sets to avoid O(n²) with Where and Any inside in-memory filtering
        var newUniqueViews = parsedViews
            .Where(x => !existingViews.Contains(x)) // Checking that a view is unique, excluding all existing views
            .Where(x => existingQuestionIds.Contains(x.QuestionId)) // Checking question existence
            .Where(x => x.UserId == null || existingUserIds.Contains((long)x.UserId)) // Checking user existence
            .ToArray();

        await viewRepository.CreateRangeAsync(newUniqueViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        var keysToDelete = allViewKeys.Prepend(CacheKeyHelper.GetViewQuestionsKey());
        await cache.KeysDeleteAsync(keysToDelete, false, cancellationToken);

        return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(newUniqueViews.Length));
    }

    public async Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidData(dto))
            return BaseResult.Failure(ErrorMessage.InvalidDataFormat,
                (int)ErrorCodes.InvalidDataFormat);

        // We do not check user and question existence
        // because that may increase the request processing time
        // And this request is called frequently

        var key = CacheKeyHelper.GetViewQuestionKey(dto.QuestionId);
        var value = GetViewValue(dto);

        var keyValueMap = new List<KeyValuePair<string, string>>
        {
            new(key, value),
            new(CacheKeyHelper.GetViewQuestionsKey(), key)
        };

        await cache.SetsAddAtomicallyAsync(keyValueMap, cancellationToken);

        return BaseResult.Success();
    }

    private static string GetViewValue(IncrementViewsDto dto)
    {
        if (dto.UserId != null)
            return dto.UserId.ToString()!;

        ArgumentException.ThrowIfNullOrWhiteSpace(dto.UserIp);
        ArgumentException.ThrowIfNullOrWhiteSpace(dto.UserFingerprint);
        var ip = IPAddress.Parse(dto.UserIp); //Throws an exception if dto.UserIp is not IP

        return $"{ip.ToString()}{ViewKeySeparator}{dto.UserFingerprint}";
    }

    private bool IsValidData(IncrementViewsDto dto)
    {
        return !StringHelper.AnyNullOrWhiteSpace(dto.UserIp, dto.UserFingerprint)
               && dto.UserFingerprint.HasMaxLength(_businessRules.UserFingerprintLength)
               && IPAddress.TryParse(dto.UserIp, out _);
    }

    private static bool IsValidKey(string key)
    {
        try
        {
            CacheKeyHelper.GetIdFromViewKey(key);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsValidValue(string value)
    {
        return long.TryParse(value, out _)
               || value.Split(ViewKeySeparator).Length == 2;
    }

    private static class ViewParsingHelpers
    {
        public static long ParseQuestionIdFromKey(string key)
        {
            if (!IsValidKey(key))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return CacheKeyHelper.GetIdFromViewKey(key);
        }

        public static View ParseViewFromValue(long questionId, string value)
        {
            var view = new View
            {
                QuestionId = questionId
            };

            if (!long.TryParse(value, out var userId))
            {
                var parts = GetValueParts(value);

                view.UserIp = parts[0];
                view.UserFingerprint = parts[1];
            }
            else
                view.UserId = userId;

            return view;
        }

        public static string GetKeyFromValue(string value) =>
            long.TryParse(value, out _) ? value : GetValueParts(value)[0];

        private static string[] GetValueParts(string s)
        {
            var parts = s.Split(ViewKeySeparator);

            if (parts.Length != 2
                || StringHelper.AnyNullOrWhiteSpace(parts)
                || !IPAddress.TryParse(parts[0], out _))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return parts;
        }
    }
}