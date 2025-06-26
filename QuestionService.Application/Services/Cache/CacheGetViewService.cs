using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetViewService(
    IBaseCacheRepository<View, long> cacheRepository,
    GetViewService inner,
    IOptions<RedisSettings> redisSettings) : IGetViewService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<View>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var views = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (views.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<View>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound),
                > 1 => CollectionResult<View>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound)
            };

        return CollectionResult<View>.Success(views);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedViews = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserViewsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetUsersViewsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedViews.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedViews = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionViewsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetQuestionsViewsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedViews.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }
}