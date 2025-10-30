using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
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
            vote => new VoteDto(vote.QuestionId, vote.UserId),
            voteDto => CacheKeyHelper.GetVoteKey(voteDto.QuestionId, voteDto.UserId),
            vote => GetVoteValue(vote.QuestionId, vote.UserId),
            GetVoteDtoFromValue,
            settings.TimeToLiveInSeconds
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
            CacheKeyHelper.GetVoteTypeVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _voteInner.GetVoteTypesVotesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    private static string GetVoteValue(long questionId, long userId) =>
        string.Format(VoteValuePattern, questionId, userId);

    private static VoteDto GetVoteDtoFromValue(string value)
    {
        var parts = value.Split(',');
        if (parts.Length < 2)
            throw new ArgumentException($"Invalid value format: {value}");

        var ids = parts.Select(long.Parse).ToArray();

        return new VoteDto(ids[0], ids[1]);
    }
}