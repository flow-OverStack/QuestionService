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
}