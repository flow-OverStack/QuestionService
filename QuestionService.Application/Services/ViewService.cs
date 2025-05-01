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
    private const string ViewKeysKey = "view:keys";
    private readonly BusinessRules _businessRules = businessRules.Value;


    //TODO make separate methods
    //TODO test all
    //TODO reduce code, simplify
    public async Task<BaseResult> SyncViewsToDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var keys =
            (await redisDatabase.SetStringMembersAsync(ViewKeysKey, cancellationToken: cancellationToken)).ToList();

        var keyValuesCollection = await redisDatabase.SetsMembersAsync(keys, cancellationToken: cancellationToken);

        var filteredKeyValuesCollection =
            keyValuesCollection.FilterByMaxValueOccurrences(GetKey, _businessRules.UserViewSpamThreshold);

        List<View> viewsToAdd = [];
        foreach (var kvp in filteredKeyValuesCollection)
        {
            if (!long.TryParse(kvp.Key.Replace(ViewKey, string.Empty), out var questionId))
                return BaseResult.Failure($"{ErrorMessage.InvalidDataFormat}: Received data is of invalid format",
                    (int)ErrorCodes.InvalidDataFormat);

            foreach (var value in kvp.Value)
            {
                var userId = long.TryParse(value, out var temp) ? temp : (long?)null;

                string? ip = null;
                string? fingerprint = null;
                if (userId == null)
                {
                    ip = value.Split(':')[0];
                    fingerprint = value.Split(':')[1];
                }

                if (StringHelper.AnyNullOrWhiteSpace(ip, fingerprint))
                    return BaseResult.Failure($"{ErrorMessage.InvalidDataFormat}: Received data is of invalid format",
                        (int)ErrorCodes.InvalidDataFormat);

                var view = new View
                {
                    Id = questionId,
                    UserId = userId,
                    UserIp = ip,
                    UserFingerprint = fingerprint
                };

                viewsToAdd.Add(view);
            }
        }

        var allViews = await viewRepository.GetAll().ToListAsync(cancellationToken);
        var filteredViews = viewsToAdd.Except(allViews, new ViewComparer()).ToList();

        await viewRepository.CreateRangeAsync(filteredViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        var keysToDelete = new List<RedisKey> { ViewKeysKey };
        keysToDelete.AddRange(keys.Select(x => (RedisKey)x));

        await redisDatabase.KeyDeleteAsync(keysToDelete.ToArray());

        return BaseResult.Success();

        #region Helper methods

        string GetKey(string s) => IsLong(s) ? s : s.Split(':')[0];

        bool IsLong(string s) => long.TryParse(s, out _);

        #endregion
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
            { ViewKeysKey, key }
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

        return $"{ip.ToString()}:{dto.UserFingerprint}";
    }

    private static bool IsValidFormat(IncrementViewsDto dto)
    {
        return !StringHelper.AnyNullOrWhiteSpace(dto.UserIp, dto.UserFingerprint)
               && IPAddress.TryParse(dto.UserIp, out _);
    }
}