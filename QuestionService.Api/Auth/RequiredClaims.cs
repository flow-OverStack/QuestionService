using System.Security.Claims;
using Newtonsoft.Json;

namespace QuestionService.Api.Auth;

internal class RequiredClaims
{
    [JsonProperty(ClaimTypes.NameIdentifier)]
    public long? UserId { get; set; }

    [JsonProperty(ClaimTypes.Name)] public string? Username { get; set; }

    [JsonProperty(ClaimTypes.Role)] public string[]? Roles { get; set; }

    public bool IsValid() => UserId != null && Username != null && Roles is { Length: > 0 };
}