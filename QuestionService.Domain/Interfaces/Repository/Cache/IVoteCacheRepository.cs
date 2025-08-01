using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IVoteCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of votes from the cache by their identifiers.
    ///     If any votes are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="dtos">The identifiers of the votes to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Vote>> GetByDtosAsync(IEnumerable<VoteDto> dtos, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves votes grouped by question identifiers from the cache.
    ///     If any question or votes are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="questionIds">The list of question IDs whose votes should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing a lookup-like list of question Ids to the list of votes.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves votes grouped by user identifiers from the cache.
    ///     If any user or votes are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose votes should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{Vote}" /> containing a lookup-like list of user Ids to the list of votes.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}