using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.Cache.Repositories;

public class ViewCacheRepository : IBaseCacheRepository<View, long>
{
    private readonly IBaseCacheRepository<View, long> _repository;

    public ViewCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<View, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetViewKey,
            x => x.Id.ToString(),
            long.Parse
        );
    }

    public Task<IEnumerable<View>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<View>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default) =>
        _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<View>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<View>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default
        ) =>
        _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);
}