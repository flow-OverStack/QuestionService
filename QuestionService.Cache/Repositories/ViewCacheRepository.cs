using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
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
            x => x.Id,
            CacheKeyHelper.GetViewKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
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
            CacheKeyHelper.GetQuestionViewsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _viewInner.GetQuestionsViewsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}