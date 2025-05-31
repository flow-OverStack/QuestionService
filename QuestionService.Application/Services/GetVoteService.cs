using LinqKit;
using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetVoteService(IBaseRepository<Vote> voteRepository, IBaseRepository<Question> questionRepository)
    : IGetVoteService
{
    public Task<QueryableResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var votes = voteRepository.GetAll();

        // Since there's can be no votes it is not an exception to have no votes 
        return Task.FromResult(QueryableResult<Vote>.Success(votes));
    }

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<GetVoteDto> dtos,
        CancellationToken cancellationToken = default)
    {
        var keys = dtos.ToList();

        var predicate = PredicateBuilder.New<Vote>();
        predicate = keys.Aggregate(predicate,
            (current, local) =>
                current.Or(x => x.QuestionId == local.QuestionId && x.UserId == local.UserId));

        var votes = await voteRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToListAsync(cancellationToken);

        if (!votes.Any())
            return keys.Count switch
            {
                <= 1 => CollectionResult<Vote>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
                > 1 => CollectionResult<Vote>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
            };

        return CollectionResult<Vote>.Success(votes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedVotes = await questionRepository.GetAll()
            .Where(x => questionIds.Contains(x.Id))
            .Include(x => x.Votes)
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Id, x.Votes))
            .ToListAsync(cancellationToken);

        if (!groupedVotes.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var votes = await voteRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);

        var groupedVotes = votes
            .GroupBy(x => x.UserId)
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Key, x))
            .ToList();

        if (!groupedVotes.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes);
    }
}