using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IViewDatabaseService
{
    /// <summary>
    ///     Synchronises the views from cache to database 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<SyncedViewsDto>> SyncViewsToDatabaseAsync(CancellationToken cancellationToken = default);
}