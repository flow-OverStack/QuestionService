using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetTagService
{
    /// <summary>
    ///     Gets all tags async
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<Tag>> GetAllAsync();

    /// <summary>
    ///     Gets tag by its name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<BaseResult<Tag>> GetByNameAsync(string name);

    /// <summary>
    ///     Gets tags of question by its id
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    Task<CollectionResult<Tag>> GetQuestionTags(long questionId);
}