using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.Cache.Repositories;

public class VoteCacheRepository : IBaseCacheRepository<Vote, VoteDto>
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly IBaseCacheRepository<Vote, VoteDto> _repository;

    public VoteCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<Vote, VoteDto>(
            cacheProvider,
            vote => new VoteDto(vote.QuestionId, vote.UserId),
            voteDto => CacheKeyHelper.GetVoteKey(voteDto.QuestionId, voteDto.UserId),
            vote => GetVoteValue(vote.QuestionId, vote.UserId),
            GetVoteDtoFromValue
        );
    }

    public Task<IEnumerable<Vote>> GetByIdsOrFetchAndCacheAsync(
        IEnumerable<VoteDto> ids,
        Func<IEnumerable<VoteDto>, CancellationToken, Task<IEnumerable<Vote>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default) =>
        _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Vote>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Vote>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default) =>
        _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);

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