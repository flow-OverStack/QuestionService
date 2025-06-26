using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.Cache.Repositories;

public class TagCacheRepository : IBaseCacheRepository<Tag, long>
{
    private readonly IBaseCacheRepository<Tag, long> _repository;

    public TagCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<Tag, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetTagKey,
            x => x.Id.ToString(),
            long.Parse
        );
    }

    public Task<IEnumerable<Tag>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Tag>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default) =>
        _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Tag>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Tag>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default
        ) =>
        _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);
}