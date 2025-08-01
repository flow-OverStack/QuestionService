using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface ITagCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of tags from the cache by their identifiers.
    ///     If any tags are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the tags to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Tag}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves tags grouped by tag identifiers from the cache.
    ///     If any question or tags are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="questionIds">The list of question IDs whose tags should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Tag}" /> containing a lookup-like list of question Ids to the list of tags.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default);
}