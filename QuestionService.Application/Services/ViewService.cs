using System.Net;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;
using StackExchange.Redis;

namespace QuestionService.Application.Services;

public class ViewService(IDatabase redisDatabase) : IViewService
{
    private const string ViewKey = "view:question:";
    private const string ViewKeysKey = "view:keys";

    public async Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto)
    {
        if (!IsValidFormat(dto))
            return BaseResult<QuestionViewsDto>.Failure(ErrorMessage.InvalidDataFormat,
                (int)ErrorCodes.InvalidDataFormat);

        var key = ViewKey + dto.QuestionId;
        var value = GetViewValue(dto);

        var keyValueMap = new Dictionary<string, string>
        {
            { key, value },
            { ViewKeysKey, key }
        };

        await redisDatabase.AddToSetsAsync(keyValueMap);

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