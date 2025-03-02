using Newtonsoft.Json;

namespace QuestionService.GraphQlClient.HttpModels;

internal class KeycloakTokenResponse
{
    [JsonProperty("access_token")] public string AccessToken { get; set; }
    [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
    [JsonProperty("expires_in")] public int AccessExpiresIn { get; set; }
    [JsonProperty("refresh_expires_in")] public int RefreshExpiresIn { get; set; }
}