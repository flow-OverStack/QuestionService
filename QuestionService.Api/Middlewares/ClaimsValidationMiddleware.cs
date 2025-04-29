using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using Newtonsoft.Json;
using QuestionService.Api.AuthModels;

namespace QuestionService.Api.Middlewares;

public class ClaimsValidationMiddleware(RequestDelegate next)
{
    private const string AuthorizationHeaderName = "Authorization";
    private const string SchemaName = "Bearer ";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true }) // if authorization is required by controller
        {
            if (RequiredClaimsExists(context))
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsync("Invalid claims");
            }
        }
        else
        {
            await next(context);
        }
    }

    private static bool RequiredClaimsExists(HttpContext context)
    {
        var token = context.Request.Headers[AuthorizationHeaderName].ToString().Replace(SchemaName, string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        var payload = jsonToken.Payload.SerializeToJson();

        try
        {
            var claims = JsonConvert.DeserializeObject<RequiredClaims>(payload);

            return claims != null && claims.IsValid();
        }
        catch (Exception)
        {
            return false;
        }
    }
}