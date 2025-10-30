using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.Cache.Repositories;

public class VoteTypeCacheRepository : IVoteTypeCacheRepository
{
    private readonly IBaseCacheRepository<VoteType, long> _repository;
    private readonly IGetVoteTypeService _voteTypeInner;

    public VoteTypeCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetVoteTypeService voteTypeInner)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<VoteType, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetVoteTypeKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
        );
        _voteTypeInner = voteTypeInner;
    }

    public Task<IEnumerable<VoteType>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _voteTypeInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}