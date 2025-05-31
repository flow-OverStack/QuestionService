using QuestionService.Domain.Interfaces.Entity;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IGetService<T> where T : IEntityId<long>
{
    /// <summary>
    ///     Gets all of T
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<QueryableResult<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets multiple T's by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<T>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
}