using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Tag controller
/// </summary>
/// <response code="200">If tag was added/updated/deleted</response>
/// <response code="400">If tag was not added/updated/deleted</response>
/// <response code="403">If the operation was forbidden for user</response>
/// <response code="500">If internal server error occured</response>
[Authorize(Roles = $"{nameof(Roles.Moderator)},{nameof(Roles.Admin)}")]
public class TagController(ITagService tagService) : BaseController
{
    /// <summary>
    ///     Adds a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request for adding a question:
    /// 
    ///     POST
    ///     {
    ///         "name":"string",
    ///         "description":"string"
    ///     }
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<BaseResult<TagDto>>> AddTag([FromBody] TagDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tagService.AddTagAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Updates a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request for adding a question:
    /// 
    ///     PUT
    ///     {
    ///         "name":"string",
    ///         "description":"string"
    ///     }
    /// </remarks>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<BaseResult<TagDto>>> UpdateTag([FromBody] TagDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tagService.UpdateTagAsync(dto, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Deletes a tag
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request for adding a question:
    /// 
    ///     DELETE {name}
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{name}")]
    public async Task<ActionResult<BaseResult<TagDto>>> DeleteTag(string name,
        CancellationToken cancellationToken)
    {
        var result = await tagService.DeleteTagAsync(name, cancellationToken);

        return HandleBaseResult(result);
    }
}