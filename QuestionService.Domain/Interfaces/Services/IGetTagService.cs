using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetTagService
{
    /// <summary>
    ///     Gets all tags
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Tag>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets tags by their names
    /// </summary>
    /// <param name="names"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Tag>> GetByNamesAsync(IEnumerable<string> names,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets tags of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default);
}