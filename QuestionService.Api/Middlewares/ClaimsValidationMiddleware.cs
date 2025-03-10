using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using QuestionService.Api.Auth;

namespace QuestionService.Api.Middlewares;

public class ClaimsValidationMiddleware(RequestDelegate next)
{
    const string AuthorizationHeaderName = "Authorization";
    private const string SchemaName = "Bearer ";

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            if (RequiredClaimsExists(context))
            {
                await next(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
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