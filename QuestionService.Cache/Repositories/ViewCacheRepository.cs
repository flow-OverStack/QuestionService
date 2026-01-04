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

public class ViewCacheRepository : IViewCacheRepository
{
    private readonly IBaseCacheRepository<View, long> _repository;
    private readonly IGetViewService _viewInner;

    public ViewCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetViewService viewInner)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<View, long>(
            cacheProvider,
            new CacheViewMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
        _viewInner = viewInner;
    }

    public Task<IEnumerable<View>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _viewInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserViewsKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserViewsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _viewInner.GetUsersViewsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionKey,
            CacheKeyHelper.GetQuestionViewsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _viewInner.GetQuestionsViewsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    private sealed class CacheViewMapping : ICacheEntityMapping<View, long>
    {
        public long GetId(View entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetViewKey(id);
        }

        public string GetValue(View entity)
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