using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Results;

namespace QuestionService.Domain.Interfaces.Service;

public interface ITagService
{
    /// <summary>
    ///     Creates tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> CreateTagAsync(CreateTagDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Edits tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> UpdateTagAsync(TagDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes tag by its id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TagDto>> DeleteTagAsync(long id, CancellationToken cancellationToken = default);
}