using QuestionService.Domain.Entities;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetTagService : IGetService<Tag>
{
    /// <summary>
    ///     Gets tags of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default);
}