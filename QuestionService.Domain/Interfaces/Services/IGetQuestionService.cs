using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetQuestionService : IGetService<Question>
{
    /// <summary>
    ///     Gets questions with tags by their names 
    /// </summary>
    /// <param name="tagNames"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<string, IEnumerable<Question>>>> GetQuestionsWithTags(
        IEnumerable<string> tagNames);

    /// <summary>
    ///     Gets questions of users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestions(IEnumerable<long> userIds);
}