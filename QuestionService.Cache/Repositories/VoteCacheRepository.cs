using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Interfaces;
using QuestionService.Cache.Repositories.Base;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.Cache.Repositories;

public class VoteCacheRepository : IVoteCacheRepository
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly IBaseCacheRepository<Vote, VoteDto> _repository;
    private readonly IGetVoteService _voteInner;

    public VoteCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetVoteService voteInner)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Vote, VoteDto>(
            cacheProvider,
            new VoteCacheMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
        _voteInner = voteInner;
    }

    public Task<IEnumerable<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            dtos,
            async (dtosToFetch, ct) => (await _voteInner.GetByDtosAsync(dtosToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionKey,
            CacheKeyHelper.GetQuestionVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _voteInner.GetQuestionsVotesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserVotesKey, // Key is the same because we don't cache users
            CacheKeyHelper.GetUserVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _voteInner.GetUsersVotesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetVoteTypesVotesAsync(
        IEnumerable<long> voteTypeIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            voteTypeIds,
            CacheKeyHelper.GetVoteTypeKey,
            CacheKeyHelper.GetVoteTypeVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _voteInner.GetVoteTypesVotesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    private sealed class VoteCacheMapping : ICacheEntityMapping<Vote, VoteDto>
    {
        public VoteDto GetId(Vote entity)
        {
            return new VoteDto(entity.QuestionId, entity.UserId);
        }

        public string GetKey(VoteDto id)
        {
            return CacheKeyHelper.GetVoteKey(id.QuestionId, id.UserId);
        }

        public string GetValue(Vote entity)
        {
            return string.Format(VoteValuePattern, entity.QuestionId, entity.UserId);
        }

        public VoteDto ParseIdFromKey(string key)
        {
            var ex = new ArgumentException($"Invalid key format: {key}");

            var parts = key.Split(':');
            if (parts.Length < 2)
                throw ex;

            parts = parts[1].Split(',');
            if (parts.Length < 2)
                throw ex;

            var ids = parts.Select(long.Parse).ToArray();
            return new VoteDto(ids[0], ids[1]);
        }

        public VoteDto ParseIdFromValue(string value)
        {
            var parts = value.Split(',');
            if (parts.Length < 2)
                throw new ArgumentException($"Invalid value format: {value}");

            var ids = parts.Select(long.Parse).ToArray();

            return new VoteDto(ids[0], ids[1]);
        }
    }
}