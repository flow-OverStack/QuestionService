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

public class CacheGetTagService : IGetTagService
{
    private readonly IBaseCacheRepository<Tag, long> _cacheRepository;
    private readonly IGetTagService _inner;
    private readonly RedisSettings _redisSettings;

    public CacheGetTagService(GetTagService inner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<Tag, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetTagKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<Tag>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _inner.GetAllAsync(cancellationToken);

    public Task<CollectionResult<Tag>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            ids,
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );


    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionTagsKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetQuestionsTagsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
}