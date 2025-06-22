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

public class CacheGetQuestionService : IGetQuestionService
{
    private readonly IBaseCacheRepository<Question, long> _cacheRepository;
    private readonly IGetQuestionService _inner;
    private readonly RedisSettings _redisSettings;

    public CacheGetQuestionService(GetQuestionService inner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<Question, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetQuestionKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _inner.GetAllAsync(cancellationToken);

    public Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            ids,
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds, CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            tagIds,
            CacheKeyHelper.GetTagQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetQuestionsWithTagsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetUsersQuestionsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
}