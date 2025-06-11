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
using StackExchange.Redis;

namespace QuestionService.Application.Services;

public class ViewService(
    IDatabase redisDatabase,
    IBaseRepository<Question> questionRepository,
    IBaseRepository<View> viewRepository,
    IEntityProvider<UserDto> userProvider,
    IOptions<BusinessRules> businessRules)
    : IViewService, IViewDatabaseService
{
    private const string ViewKey = "view:question:";
    private const string ViewsKeysKey = "view:keys";
    private const char ViewKeySeparator = ',';
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<SyncedViewsDto>> SyncViewsToDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        #region Removing invalid keys

        var allViewKeys =
            (await redisDatabase.SetStringMembersAsync(ViewsKeysKey, cancellationToken)).ToList();
        var invalidKeys = await RemoveInvalidViewKeysAsync(allViewKeys, cancellationToken);
        var validViewKeys = allViewKeys.Except(invalidKeys);

        #endregion

        #region Removing invalid values

        var allViewValues =
            (await redisDatabase.SetsMembersAsync(validViewKeys, cancellationToken)).ToList();
        var invalidViewValues = await RemoveInvalidViewValuesAsync(allViewValues, cancellationToken);
        var validViewValues = allViewValues.FilterByInvalidValues(invalidViewValues);

        #endregion

        var spamFilteredViews = validViewValues.FilterByMaxValueOccurrences(ViewParsingHelpers.GetKeyFromValue,
            _businessRules.UserViewSpamThreshold);

        var parsedViews = spamFilteredViews
            .SelectManyFromGroupedValues(ViewParsingHelpers.ParseQuestionIdFromKey,
                ViewParsingHelpers.ParseViewFromValue).ToList();

        if (parsedViews.Count == 0) return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(0));

        #region Filtering views to have real user and question ids and to be unique

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
            .Where(x => !existingViews.Contains(x)) // Checking that view is unique, excluding all existing views
            .Where(x => existingQuestionIds.Contains(x.QuestionId)) // Checking question existence
            .Where(x => x.UserId == null || existingUserIds.Contains((long)x.UserId)) // Checking user existence
            .ToList();

        #endregion

        await viewRepository.CreateRangeAsync(newUniqueViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        var keysToDelete = allViewKeys.Prepend(ViewsKeysKey).Select(k => (RedisKey)k).ToArray();
        await redisDatabase.KeyDeleteAsync(keysToDelete);

        return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(newUniqueViews.Count));
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

        var key = ViewKey + dto.QuestionId;
        var value = GetViewValue(dto);

        var keyValueMap = new List<KeyValuePair<string, string>>
        {
            new(key, value),
            new(ViewsKeysKey, key)
        };

        await redisDatabase.SetsAddAtomicallyAsync(keyValueMap, cancellationToken);

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

    private async Task<IEnumerable<string>> RemoveInvalidViewKeysAsync(IEnumerable<string> allViewKeys,
        CancellationToken cancellationToken = default)
    {
        var invalidValues = allViewKeys.Where(x => !IsValidKey(x)).Select(x => new RedisValue(x)).ToArray();

        cancellationToken.ThrowIfCancellationRequested();
        await redisDatabase.SetRemoveAsync(new RedisKey(ViewsKeysKey), invalidValues);

        return invalidValues.Select(x => x.ToString());
    }

    private async Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> RemoveInvalidViewValuesAsync(
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> allViewValues,
        CancellationToken cancellationToken = default)
    {
        var invalidViewValues = allViewValues.Select(x =>
            new KeyValuePair<RedisKey, IEnumerable<RedisValue>>(x.Key,
                x.Value.Where(y => !IsValidValue(y)).Select(y => new RedisValue(y)))).ToList();
        await redisDatabase.SetsRemoveAsync(invalidViewValues, cancellationToken);

        return invalidViewValues.Select(x =>
            new KeyValuePair<string, IEnumerable<string>>(x.Key.ToString(), x.Value.Select(y => y.ToString())));
    }

    private static bool IsValidKey(string key)
    {
        return long.TryParse(key.Replace(ViewKey, string.Empty), out _);
    }

    private static bool IsValidValue(string value)
    {
        return long.TryParse(value, out _)
               || value.Split(ViewKeySeparator).Length == 2;
    }

    private static class ViewParsingHelpers
    {
        public static long ParseQuestionIdFromKey(string rawKey)
        {
            if (!long.TryParse(rawKey.Replace(ViewKey, string.Empty), out var questionId))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return questionId;
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