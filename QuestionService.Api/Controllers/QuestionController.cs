using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Dtos.Request;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Question controller
/// </summary>
/// <response code="200">If question was asked/edited/deleted</response>
/// <response code="400">If question was not asked/edited/deleted</response>
/// <response code="403">If the operation was forbidden for user</response>
/// <response code="500">If internal server error occured</response>
[Authorize]
public class QuestionController(IQuestionService questionService) : BaseController
{
    /// <summary>
    ///     Creates a question
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for ask question:
    /// 
    ///     POST
    ///     {
    ///         "title":"string",
    ///         "body":"string",
    ///         "tagNames":[
    ///            "string"
    ///          ]
    ///     }
    /// </remarks>
    [HttpPost]
    public async Task<ActionResult<BaseResult<QuestionDto>>> AskQuestion(AskQuestionDto dto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var result = await questionService.AskQuestionAsync(userId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    ///     Deletes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for delete question:
    /// 
    ///     DELETE {questionId}
    /// </remarks>
    [HttpDelete("{questionId:long}")]
    public async Task<ActionResult<BaseResult<QuestionDto>>> DeleteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DeleteQuestionAsync(userId, questionId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    ///     Edits a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for edit question:
    /// 
    ///     PUT
    ///     {
    ///         "id":0
    ///         "title":"string",
    ///         "body":"string",
    ///         "tagNames":[
    ///            "string"
    ///          ]
    ///     } 
    /// </remarks>
    [HttpPut("{questionId:long}")]
    public async Task<ActionResult<BaseResult<QuestionDto>>> EditQuestion(long questionId,
        RequestEditQuestionDto requestDto,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var dto = new EditQuestionDto(questionId, requestDto.Title, requestDto.Body, requestDto.TagNames);

        var result = await questionService.EditQuestionAsync(userId, dto, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    ///     Downvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for downvote question:
    /// 
    ///     PATCH downvote/{questionId}
    /// </remarks>
    [HttpPatch("downvote/{questionId:long}")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> DownvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DownvoteQuestionAsync(userId, questionId, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    ///     Upvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for upvote question:
    /// 
    ///     PATCH upvote/{questionId}
    /// </remarks>
    [HttpPatch("upvote/{questionId:long}")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> UpvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.UpvoteQuestionAsync(userId, questionId, cancellationToken);

        return HandleResult(result);
    }
}