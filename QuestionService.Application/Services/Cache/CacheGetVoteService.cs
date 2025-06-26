using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteService(
    IBaseCacheRepository<Vote, VoteDto> cacheRepository,
    GetVoteService inner,
    IOptions<RedisSettings> redisSettings) : IGetVoteService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var dtosArray = dtos.ToArray();
        var votes = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            dtosArray,
            async (dtosToFetch, ct) => (await inner.GetByDtosAsync(dtosToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (votes.Length == 0)
            return dtosArray.Length switch
            {
                <= 1 => CollectionResult<Vote>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
                > 1 => CollectionResult<Vote>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
            };

        return CollectionResult<Vote>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetQuestionsVotesAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserVotesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetUsersVotesAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }
}