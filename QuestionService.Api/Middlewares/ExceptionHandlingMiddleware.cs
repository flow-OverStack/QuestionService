using System.Net;
using System.Net.Mime;
using QuestionService.Application.Resources;
using QuestionService.Domain.Results;
using ILogger = Serilog.ILogger;

namespace QuestionService.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        logger.Error(exception, "Error: {ErrorMessage}. Path: {Path}. Method: {Method}. IP: {IP}",
            exception.Message.TrimEnd('.'),
            httpContext.Request.Path, httpContext.Request.Method, httpContext.Connection.RemoteIpAddress);

        // We return nothing because the request is already canceled 
        if (exception is OperationCanceledException) return;

        var (message, statusCode) = exception switch
        {
            _ => ($"{ErrorMessage.InternalServerError}: {exception.Message}", (int)HttpStatusCode.InternalServerError)
        };
        var response = BaseResult.Failure(message, statusCode);


        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = response.ErrorCode ?? (int)HttpStatusCode.InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}