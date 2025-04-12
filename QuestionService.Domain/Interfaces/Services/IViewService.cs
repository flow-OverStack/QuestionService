using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IViewService
{
    /// <summary>
    ///     Increments views of question by one view
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto);
}