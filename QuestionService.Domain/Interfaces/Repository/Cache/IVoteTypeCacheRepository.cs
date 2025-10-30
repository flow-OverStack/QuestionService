using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces.Repository.Cache;

public interface IVoteTypeCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of VoteType entities based on the given identifiers.
    /// </summary>
    /// <param name="ids">A collection of long identifiers for the VoteType entities to retrieve.</param>
    /// <param name="cancellationToken">An optional CancellationToken to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an enumerable of VoteType entities.</returns>
    Task<IEnumerable<VoteType>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}