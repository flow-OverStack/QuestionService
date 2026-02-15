using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Api.Dtos;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     Question controller
/// </summary>
/// <response code="200">If question was asked/edited/deleted</response>
/// <response code="400">If question was not asked/edited/deleted</response>
/// <response code="403">If the operation was forbidden for user</response>
/// <response code="500">If internal server error occurred</response>
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
    /// Request to ask a question:
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

        return HandleBaseResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    ///     Deletes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to delete a question:
    /// 
    ///     DELETE {questionId}
    /// </remarks>
    [HttpDelete("{questionId:long}")]
    public async Task<ActionResult<BaseResult<QuestionDto>>> DeleteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DeleteQuestionAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Edits a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="requestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to edit a question:
    /// 
    ///     PUT
    ///     {
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

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Downvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to downvote a question:
    /// 
    ///     PATCH {questionId}/downvote
    /// </remarks>
    [HttpPatch("{questionId:long}/downvote")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> DownvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DownvoteQuestionAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Upvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request to upvote a question:
    /// 
    ///     PATCH {questionId}/upvote
    /// </remarks>
    [HttpPatch("{questionId:long}/upvote")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> UpvoteQuestion(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.UpvoteQuestionAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }

    /// <summary>
    ///     Removes user's vote from a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     Request to remove a vote from a question:
    ///     DELETE {questionId}/vote
    /// </remarks>
    [HttpDelete("{questionId:long}/vote")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> RemoveQuestionVote(long questionId,
        CancellationToken cancellationToken)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.RemoveQuestionVoteAsync(userId, questionId, cancellationToken);

        return HandleBaseResult(result);
    }
}