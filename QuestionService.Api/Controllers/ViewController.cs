using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Controllers.Base;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers;

/// <summary>
///     View controller
/// </summary>
/// <response code="200">If views were incremented</response>
/// <response code="400">If views were not incremented</response>
/// <response code="422">If some request data (eg userIp) was not provided</response>
/// <response code="500">If internal server error occured</response>
public class ViewController(IViewService viewService) : BaseController
{
    private const string FingerprintHeaderName = "X-Fingerprint";

    /// <summary>
    ///     Increments views of a question by its id
    /// </summary>
    /// <param name="questionId"></param>
    /// <param name="fingerprint"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <remarks>
    ///     POST {questionId}
    /// </remarks>
    [HttpPost("{questionId:long}")]
    public async Task<ActionResult<BaseResult>> IncrementViews(long questionId, CancellationToken cancellationToken,
        [FromHeader(Name = FingerprintHeaderName)]
        string? fingerprint = null)
    {
        if (!TryGetUserIp(out var userIp)) return UnprocessableEntity("IP Address is not provided");
        if (string.IsNullOrWhiteSpace(fingerprint)) return UnprocessableEntity("Fingerprint is not provided");

        var userId = GetUserIdIfExists();

        var dto = new IncrementViewsDto(questionId, userId, userIp, fingerprint);

        var result = await viewService.IncrementViewsAsync(dto, cancellationToken);

        return HandleResult(result);
    }

    private long? GetUserIdIfExists()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!long.TryParse(userIdClaim, out var userId)) return null;

        return userId;
    }

    /// <summary>
    /// IMPORTANT: User IP depends on KnownProxies in config.
    /// Set KnownProxies to access real user IP.
    /// Known proxies are known reverse proxies or load balancers.
    /// Known reverse proxies or load balancers are assumed to remove custom X-Forwarded-For headers before sending request to this server.
    /// </summary>
    /// <param name="userIp"></param>
    /// <returns></returns>
    private bool TryGetUserIp([MaybeNullWhen(false)] out string userIp)
    {
        userIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        return !string.IsNullOrEmpty(userIp);
    }
}