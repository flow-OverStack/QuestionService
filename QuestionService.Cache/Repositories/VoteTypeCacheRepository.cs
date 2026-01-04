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
            new CacheVoteTypeMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
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

    private sealed class CacheVoteTypeMapping : ICacheEntityMapping<VoteType, long>
    {
        public long GetId(VoteType entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetVoteTypeKey(id);
        }

        public string GetValue(VoteType entity)
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