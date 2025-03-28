using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetVoteService
{
    /// <summary>
    ///     Get all votes
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<Vote>> GetAllAsync();

    /// <summary>
    ///     Get Vote of question by questionId and userId
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<BaseResult<Vote>> GetByIdsAsync(long questionId, long userId);

    /// <summary>
    ///     Gets votes of question by its id
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<CollectionResult<Vote>> GetQuestionVotesAsync(long questionId);
}