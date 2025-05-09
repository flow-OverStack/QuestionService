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
    private const string ViewKeySeparator = "_";
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<SyncedViewsDto>> SyncViewsToDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        var allViewKeys =
            (await redisDatabase.SetStringMembersAsync(ViewsKeysKey, cancellationToken)).ToList();

        var viewEntriesByKey =
            (await redisDatabase.SetsMembersAsync(allViewKeys, cancellationToken)).ToList();

        var invalidViewEntries = viewEntriesByKey.Select(x =>
            new KeyValuePair<RedisKey, IEnumerable<RedisValue>>(x.Key,
                x.Value.Where(y => !IsValidKey(y)).Select(y => new RedisValue(y))));
        await redisDatabase.SetsRemoveAsync(invalidViewEntries, cancellationToken);

        var validViewEntries = viewEntriesByKey.Select(x =>
            new KeyValuePair<string, IEnumerable<string>>(x.Key,
                x.Value.Where(IsValidKey)));

        var spamFilteredViews = validViewEntries.FilterByMaxValueOccurrences(ViewParsingHelpers.GetKeyFromValue,
            _businessRules.UserViewSpamThreshold);

        var parsedViews = spamFilteredViews
            .SelectManyFromGroupedValues(ViewParsingHelpers.ParseQuestionIdFromKey,
                ViewParsingHelpers.ParseViewFromValue).ToList();

        if (!parsedViews.Any()) return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(0));

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

        await viewRepository.CreateRangeAsync(newUniqueViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        var keysToDelete = allViewKeys.Prepend(ViewsKeysKey).Select(k => (RedisKey)k).ToArray();
        await redisDatabase.KeyDeleteAsync(keysToDelete);

        return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(newUniqueViews.Count));
    }

    public async Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidFormat(dto))
            return BaseResult.Failure(ErrorMessage.InvalidDataFormat,
                (int)ErrorCodes.InvalidDataFormat);

        // We do not check user and question existence
        // because that may increase the request processing time
        // And this request is called frequently

        var key = ViewKey + dto.QuestionId;
        var value = GetViewValue(dto);

        var keyValueMap = new Dictionary<string, string>
        {
            { key, value },
            { ViewsKeysKey, key }
        };

        await redisDatabase.SetsAddAsync(keyValueMap, cancellationToken);

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

    private static bool IsValidFormat(IncrementViewsDto dto)
    {
        return !StringHelper.AnyNullOrWhiteSpace(dto.UserIp, dto.UserFingerprint)
               && IPAddress.TryParse(dto.UserIp, out _);
    }

    private static bool IsValidKey(string key)
    {
        return long.TryParse(key, out _)
               || key.Split(ViewKeySeparator).Length == 2;
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
            string? ip = null, fingerprint = null;
            if (!long.TryParse(value, out var userId))
            {
                var parts = GetValueParts(value);

                ip = parts[0];
                fingerprint = parts[1];
            }

            var view = new View
            {
                QuestionId = questionId,
                UserId = userId != 0 ? userId : null,
                UserIp = ip,
                UserFingerprint = fingerprint
            };

            return view;
        }

        public static string GetKeyFromValue(string value) => IsLong(value) ? value : GetValueParts(value)[0];

        private static string[] GetValueParts(string s)
        {
            var parts = s.Split(ViewKeySeparator);

            if (parts.Length != 2
                || StringHelper.AnyNullOrWhiteSpace(parts)
                || !IPAddress.TryParse(parts[0], out _))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return parts;
        }

        private static bool IsLong(string s) => long.TryParse(s, out _);
    }
}