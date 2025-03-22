using System.Net;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;
using ILogger = Serilog.ILogger;

namespace QuestionService.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);

            switch (httpContext.Response.StatusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    httpContext.Response.ContentType = "text/plain";
                    var message = $"{(int)HttpStatusCode.NotFound} {nameof(HttpStatusCode.NotFound)}\nPlease check URL";
                    await httpContext.Response.WriteAsync(message);
                    break;
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        logger.Error(exception, "Error: {errorMessage}. Path: {Path}. Method: {Method}. IP: {IP}", exception.Message,
            httpContext.Request.Path, httpContext.Request.Method, httpContext.Connection.RemoteIpAddress);


        var response = exception switch
        {
            _ => BaseResult.Failure($"{ErrorMessage.InternalServerError}: {exception.Message}",
                (int)HttpStatusCode.InternalServerError)
        };


        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)response.ErrorCode!;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}