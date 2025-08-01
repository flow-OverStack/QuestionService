using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
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
            x => x.Id,
            CacheKeyHelper.GetQuestionKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
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
            CacheKeyHelper.GetUserQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _questionInner.GetUsersQuestionsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}