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
    Task<BaseResult<QuestionDto>> AskQuestion(long initiatorId, AskQuestionDto dto);

    /// <summary>
    /// Edits question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> EditQuestion(long initiatorId, EditQuestionDto dto);

    /// <summary>
    /// Deletes the question
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<QuestionDto>> DeleteQuestion(long initiatorId, long questionId);

    /// <summary>
    /// Increases question's reputation by 1 
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> UpvoteQuestion(long initiatorId, long questionId);

    /// <summary>
    /// Decreases question's reputation by 1 
    /// </summary>
    /// <param name="initiatorId">Id of request initiator (e.g. Id of user or moderator)</param>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<BaseResult<VoteQuestionDto>> DownvoteQuestion(long initiatorId, long questionId);
}