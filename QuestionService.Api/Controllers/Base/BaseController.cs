using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Results;

namespace QuestionService.Api.Controllers.Base;

/// <inheritdoc />
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class BaseController : ControllerBase
{
    /// <summary>
    ///     Handles the BaseResult of type T and returns the corresponding ActionResult 
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T">Type of BaseResult</typeparam>
    /// <returns></returns>
    protected ActionResult<BaseResult<T>> HandleBaseResult<T>(BaseResult<T> result)
    {
        return result.ErrorCode switch
        {
            (int)ErrorCodes.OperationForbidden => Forbid(),
            _ when !result.IsSuccess => BadRequest(result),
            _ => Ok(result)
        };
    }

    /// <summary>
    ///     Handles the BaseResult and returns the corresponding ActionResult 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    protected ActionResult<BaseResult> HandleBaseResult(BaseResult result)
    {
        return result.ErrorCode switch
        {
            (int)ErrorCodes.OperationForbidden => Forbid(),
            _ when !result.IsSuccess => BadRequest(result),
            _ => Ok(result)
        };
    }
}