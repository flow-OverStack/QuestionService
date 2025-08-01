using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetVoteService(IVoteCacheRepository cacheRepository, GetVoteService inner) : IGetVoteService
{
    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var dtosArray = dtos.ToArray();
        var votes = (await cacheRepository.GetByDtosAsync(dtosArray, cancellationToken)).ToArray();

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
        var groupedVotes = (await cacheRepository.GetQuestionsVotesAsync(questionIds, cancellationToken)).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedVotes = (await cacheRepository.GetUsersVotesAsync(userIds, cancellationToken)).ToArray();

        if (groupedVotes.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }
}