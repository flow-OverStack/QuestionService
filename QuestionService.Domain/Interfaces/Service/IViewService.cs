using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface IViewService
{
    /// <summary>
    ///     Increments views of question by one view
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto, CancellationToken cancellationToken = default);
}