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

    public async Task<BaseResult<Vote>> GetByIdsAsync(GetVoteDto dto)
    {
        var question = await questionRepository.GetAll().Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == dto.QuestionId);

        if (question == null)
            return BaseResult<Vote>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == dto.UserId);

        if (vote == null)
            return BaseResult<Vote>.Failure(ErrorMessage.VoteNotFound, (int)ErrorCodes.VoteNotFound);

        return BaseResult<Vote>.Success(vote);
    }

    public async Task<CollectionResult<Vote>> GetQuestionVotesAsync(long questionId)
    {
        var question = await questionRepository.GetAll().Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (question == null)
            return CollectionResult<Vote>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        var votes = question.Votes;

        return CollectionResult<Vote>.Success(votes, votes.Count);
    }
}