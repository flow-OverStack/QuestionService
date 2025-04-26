using LinqKit;
using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetVoteService(IBaseRepository<Vote> voteRepository, IBaseRepository<Question> questionRepository)
    : IGetVoteService
{
    public async Task<CollectionResult<Vote>> GetAllAsync()
    {
        var votes = await voteRepository.GetAll().ToListAsync();

        // Since there's can be no votes it is not an exception to have no votes 
        return CollectionResult<Vote>.Success(votes, votes.Count);
    }

    public async Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<GetVoteDto> dtos)
    {
        var keys = dtos.ToList();

        var predicate = PredicateBuilder.New<Vote>();
        predicate = keys.Aggregate(predicate,
            (current, local) =>
                current.Or(x => x.QuestionId == local.QuestionId && x.UserId == local.UserId));

        var votes = await voteRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToListAsync();
        var totalCount = await voteRepository.GetAll().CountAsync();

        if (!votes.Any())
            return keys.Count switch
            {
                <= 1 => CollectionResult<Vote>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound),
                > 1 => CollectionResult<Vote>.Failure(ErrorMessage.VotesNotFound, (int)ErrorCodes.VotesNotFound)
            };

        return CollectionResult<Vote>.Success(votes, votes.Count, totalCount);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(
        IEnumerable<long> questionIds)
    {
        var groupedVotes = await questionRepository.GetAll()
            .Where(x => questionIds.Contains(x.Id))
            .Include(x => x.Votes)
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Id, x.Votes))
            .ToListAsync();

        if (!groupedVotes.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes, groupedVotes.Count);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(
        IEnumerable<long> userIds)
    {
        var votes = await voteRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync();

        var groupedVotes = votes
            .GroupBy(x => x.UserId)
            .Select(x => new KeyValuePair<long, IEnumerable<Vote>>(x.Key, x))
            .ToList();

        if (!groupedVotes.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Failure(ErrorMessage.VotesNotFound,
                (int)ErrorCodes.VotesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>.Success(groupedVotes, groupedVotes.Count);
    }
}