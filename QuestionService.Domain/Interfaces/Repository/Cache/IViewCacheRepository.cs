using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IViewCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of views from the cache by their identifiers.
    ///     If any views are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the views to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{View}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<View>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves views grouped by user identifiers from the cache.
    ///     If any user or views are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose views should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{View}" /> containing a lookup-like list of user Ids to the list of views.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves views grouped by question identifiers from the cache.
    ///     If any question or views are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="questionIds">The list of question IDs whose views should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{View}" /> containing a lookup-like list of question Ids to the list of views.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default);
}