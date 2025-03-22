using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Result;

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
    public async Task<ActionResult<BaseResult<QuestionDto>>> AskQuestion(AskQuestionDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.AskQuestion(userId, dto);

        return HandleResult(result);
    }

    /// <summary>
    ///     Deletes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for delete question:
    /// 
    ///     DELETE {questionId}
    /// </remarks>
    [HttpDelete("{questionId:long}")]
    public async Task<ActionResult<BaseResult<QuestionDto>>> DeleteQuestion(long questionId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DeleteQuestion(userId, questionId);

        return HandleResult(result);
    }

    /// <summary>
    ///     Edits a question
    /// </summary>
    ///  <param name="dto"></param>
    ///  <returns></returns>
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
    [HttpPut]
    public async Task<ActionResult<BaseResult<QuestionDto>>> EditQuestion(EditQuestionDto dto)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.EditQuestion(userId, dto);

        return HandleResult(result);
    }

    /// <summary>
    ///     Downvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for downvote question:
    /// 
    ///     PATCH downvote/{questionId}
    /// </remarks>
    [HttpPatch("downvote/{questionId:long}")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> DownvoteQuestion(long questionId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.DownvoteQuestion(userId, questionId);

        return HandleResult(result);
    }

    /// <summary>
    ///     Upvotes a question
    /// </summary>
    /// <param name="questionId"></param>
    /// <returns></returns>
    /// <remarks>
    /// Request for upvote question:
    /// 
    ///     PATCH upvote/{questionId}
    /// </remarks>
    [HttpPatch("upvote/{questionId:long}")]
    public async Task<ActionResult<BaseResult<VoteQuestionDto>>> UpvoteQuestion(long questionId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await questionService.UpvoteQuestion(userId, questionId);

        return HandleResult(result);
    }
}