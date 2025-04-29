using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface ITagService
{
    /// <summary>
    ///     Adds tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> AddTagAsync(TagDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Edits tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> UpdateTagAsync(TagDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes tag by its name
    /// </summary>
    /// <param name="tagName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> DeleteTagAsync(string tagName, CancellationToken cancellationToken = default);
}