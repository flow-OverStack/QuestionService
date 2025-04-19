using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetViewService : IGetService<View>
{
    /// <summary>
    ///     Gets questions of users by their ids 
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(IEnumerable<long> userIds);

    /// <summary>
    ///     Gets views of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(IEnumerable<long> questionIds);
}