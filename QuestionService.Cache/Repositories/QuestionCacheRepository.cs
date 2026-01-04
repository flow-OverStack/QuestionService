using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Cache.Repositories.Base;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.Cache.Repositories;

public class QuestionCacheRepository : IQuestionCacheRepository
{
    private readonly IGetQuestionService _questionInner;
    private readonly IBaseCacheRepository<Question, long> _repository;

    public QuestionCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetQuestionService questionInner)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Question, long>(
            cacheProvider,
            new CacheQuestionMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
        _questionInner = questionInner;
    }

    public Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _questionInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            tagIds,
            CacheKeyHelper.GetTagKey,
            CacheKeyHelper.GetTagQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _questionInner.GetQuestionsWithTagsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserQuestionsKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _questionInner.GetUsersQuestionsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    private sealed class CacheQuestionMapping : ICacheEntityMapping<Question, long>
    {
        public long GetId(Question entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetQuestionKey(id);
        }

        public string GetValue(Question entity)
        {
            return entity.Id.ToString();
        }

        public long ParseIdFromKey(string key)
        {
            return CacheKeyHelper.GetIdFromKey(key);
        }

        public long ParseIdFromValue(string value)
        {
            return long.Parse(value);
        }
    }
}