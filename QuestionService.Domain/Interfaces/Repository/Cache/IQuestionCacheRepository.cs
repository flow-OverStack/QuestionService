using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IQuestionCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of questions from the cache by their identifiers.
    ///     If any questions are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the questions to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Question}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves questions grouped by tag identifiers from the cache.
    ///     If any tag or questions are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="tagIds">The list of tag IDs whose questions should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Question}" /> containing a lookup-like list of tag Ids to the list of questions.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves questions grouped by user identifiers from the cache.
    ///     If any user or questions are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose questions should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Question}" /> containing a lookup-like list of user Ids to the list of questions.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default);
}