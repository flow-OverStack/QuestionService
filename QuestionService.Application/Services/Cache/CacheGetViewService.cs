using Microsoft.Extensions.Options;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetViewService : IGetViewService
{
    private readonly IBaseCacheRepository<View, long> _cacheRepository;
    private readonly IGetViewService _inner;
    private readonly RedisSettings _redisSettings;

    public CacheGetViewService(GetViewService inner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<View, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetViewKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _inner.GetAllAsync(cancellationToken);

    public Task<CollectionResult<View>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            ids,
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserViewsKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetUsersViewsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionViewsKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetQuestionsViewsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
}