using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IQuestionService
{
    /// <summary>
    /// Creates a question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto);

    /// <summary>
    /// Edits question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> EditQuestionAsync(long initiatorId, EditQuestionDto dto);

    /// <summary>
    /// Deletes the question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> DeleteQuestionAsync(long initiatorId, long questionId);

    /// <summary>
    /// Increases question's reputation by 1 
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> UpvoteQuestionAsync(long initiatorId, long questionId);

    /// <summary>
    /// Decreases question's reputation by 1 
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> DownvoteQuestionAsync(long initiatorId, long questionId);
}