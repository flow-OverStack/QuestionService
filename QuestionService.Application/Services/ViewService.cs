using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Comparers;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
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
    IOptions<BusinessRules> businessRules)
    : IViewService, IViewDatabaseService
{
    private const string ViewKey = "view:question:";
    private const string ViewsKeysKey = "view:keys";
    private const string ViewKeySeparator = "_";
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult> SyncViewsToDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var allViewKeys =
            (await redisDatabase.SetStringMembersAsync(ViewsKeysKey, cancellationToken)).ToList();

        var viewEntriesByKey =
            (await redisDatabase.SetsMembersAsync(allViewKeys, cancellationToken)).ToList();

        var invalidViewEntries = viewEntriesByKey.Select(x =>
            new KeyValuePair<RedisKey, IEnumerable<RedisValue>>(x.Key,
                x.Value.Where(y => y.Split(ViewKeySeparator).Length != 2).Select(y => new RedisValue(y))));
        await redisDatabase.SetsRemoveAsync(invalidViewEntries, cancellationToken);

        var validViewEntries = viewEntriesByKey.Select(x =>
            new KeyValuePair<string, IEnumerable<string>>(x.Key,
                x.Value.Where(y => y.Split(ViewKeySeparator).Length == 2)));

        var spamFilteredViews = validViewEntries.FilterByMaxValueOccurrences(ViewParsingHelpers.GetKeyFromValue,
            _businessRules.UserViewSpamThreshold);

        var parsedViews = spamFilteredViews
            .SelectManyFromGroupedValues(ViewParsingHelpers.ParseQuestionIdFromKey,
                ViewParsingHelpers.ParseViewFromValue).ToList();

        var existingViews = await viewRepository.GetAll().ToListAsync(cancellationToken);
        var newUniqueViews = parsedViews.Except(existingViews, new UniqueViewComparer()).ToList();

        await viewRepository.CreateRangeAsync(newUniqueViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        var keysToDelete = allViewKeys.Prepend(ViewsKeysKey).Select(k => (RedisKey)k).ToArray();

        await redisDatabase.KeyDeleteAsync(keysToDelete);

        return BaseResult.Success();
    }

    public async Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidFormat(dto))
            return BaseResult.Failure(ErrorMessage.InvalidDataFormat,
                (int)ErrorCodes.InvalidDataFormat);

        var questionExists = await questionRepository.GetAll().AnyAsync(x => x.Id == dto.QuestionId, cancellationToken);
        if (!questionExists)
            return BaseResult.Failure(ErrorMessage.QuestionNotFound,
                (int)ErrorCodes.QuestionNotFound);

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

            if (parts.Length != 2 || StringHelper.AnyNullOrWhiteSpace(parts))
                throw new FormatException(ErrorMessage.InvalidCacheDataFormat);

            return parts;
        }

        private static bool IsLong(string s) => long.TryParse(s, out _);
    }
}