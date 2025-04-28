using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

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