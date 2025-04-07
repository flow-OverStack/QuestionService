using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetQuestionService : IGetService<Question>
{
    /// <summary>
    ///     Gets questions with tag by its name 
    /// </summary>
    /// <param name="tagName"></param>
    /// <returns></returns>
    Task<CollectionResult<Question>> GetQuestionsWithTag(string tagName);

    /// <summary>
    ///     Gets questions of user by its id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CollectionResult<Question>> GetUserQuestions(long userId);

    Task<BaseResult<QuestionViewsDto>> GetQuestionViewsCount(long questionId);
}