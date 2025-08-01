using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IViewCacheSyncRepository
{
    /// <summary>
    ///     Retrieves all valid views stored in the cache, optionally filtering out repeated values
    ///     that exceed a spam threshold (e.g., same IP or user spamming views).
    /// </summary>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A collection of <see cref="IEnumerable{View}" /> objects parsed from the cache.</returns>
    Task<IEnumerable<View>> GetValidViewsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all cache entries related to view tracking, including individual view sets and the index of view keys.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAllViewsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds a new view entry to the cache, associating a question with a user ID or a fingerprint + IP address
    ///     combination.
    /// </summary>
    /// <param name="dto">The data transfer object containing information about the view to cache.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous add operation.</returns>
    Task AddViewAsync(IncrementViewsDto dto, CancellationToken cancellationToken = default);
}