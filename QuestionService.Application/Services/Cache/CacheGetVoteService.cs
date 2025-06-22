using Microsoft.Extensions.Options;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteService : IGetVoteService
{
    private const string VoteValuePattern = "{0},{1}";

    private readonly IBaseCacheRepository<Vote, VoteDto> _cacheRepository;
    private readonly IGetVoteService _inner;
    private readonly RedisSettings _redisSettings;

    public CacheGetVoteService(GetVoteService inner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<Vote, VoteDto>(
            cacheProvider,
            vote => new VoteDto(vote.QuestionId, vote.UserId),
            voteDto => CacheKeyHelper.GetVoteKey(voteDto.QuestionId, voteDto.UserId),
            vote => GetVoteValue(vote.QuestionId, vote.UserId),
            GetVoteDtoFromValue
        );
        _inner = inner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default) =>
        _inner.GetAllAsync(cancellationToken);

    public Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            dtos,
            _inner.GetByDtosAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionVotesKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetQuestionsVotesAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default) =>
        _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserVotesKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetUsersVotesAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );

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