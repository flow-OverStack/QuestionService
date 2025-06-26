using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.Cache.Repositories;

public class QuestionCacheRepository : IBaseCacheRepository<Question, long>
{
    private readonly IBaseCacheRepository<Question, long> _repository;

    public QuestionCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<Question, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetQuestionKey,
            x => x.Id.ToString(),
            long.Parse
        );
    }

    public Task<IEnumerable<Question>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Question>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default) =>
        _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Question>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Question>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default
        ) =>
        _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);
}