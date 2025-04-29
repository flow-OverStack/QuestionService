using QuestionService.Domain.Entities;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetQuestionService : IGetService<Question>
{
    /// <summary>
    ///     Gets questions with tags by their names 
    /// </summary>
    /// <param name="tagNames"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<string, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<string> tagNames, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets questions of users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}