using System.Net;
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
    ///     Creates a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to create a tag:
    /// 
    ///     POST
    ///     {
    ///         "name":"string",
    ///         "description":"string"
    ///     }
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<BaseResult<TagDto>>> CreateTag([FromBody] CreateTagDto dto,
        CancellationToken cancellationToken)
    {
        var result = await tagService.CreateTagAsync(dto, cancellationToken);

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Updates a tag
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to update a tag:
    /// 
    ///     PUT
    ///     {
    ///         "id":1,
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
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <remarks>
    /// Request to delete a tag:
    /// 
    ///     DELETE {id:long}
    /// </remarks>
    /// <returns></returns>
    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResult<TagDto>>> DeleteTag(long id,
        CancellationToken cancellationToken)
    {
        var result = await tagService.DeleteTagAsync(id, cancellationToken);

        return HandleBaseResult(result);
    }
}