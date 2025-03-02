using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QuestionService.Domain.Settings;
using QuestionService.GraphQlClient.HttpModels;
using QuestionService.GraphQlClient.Interfaces;

namespace QuestionService.GraphQlClient.Auth;

public class GraphQlAuthService(IHttpClientFactory httpClientFactory, IOptions<KeycloakSettings> keyclocakSettings)
    : IGraphQlAuthProvider
{
    private const int TokenExpirationThresholdInSeconds = 5;
    private const string ClientCredentialsGrantType = "client_credentials";
    
    private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
    private readonly KeycloakSettings _keycloakSettings = keyclocakSettings.Value;
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("GraphQlAuthServiceHttpClient");
    private static KeycloakServiceToken? Token { get; set; }

    public async Task<string> GetServiceTokenAsync()
    {
        await UpdateServiceTokenIfNeeded();
        return Token!.AccessToken;
    }

    private async Task UpdateServiceToken()
    {
        if (!IsTokenExpired())
            return; //double check is here to check if 2 or more threads are updating the token at the same time after the first check

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _keycloakSettings.ClientId },
            { "client_secret", _keycloakSettings.AdminToken },
            { "grant_type", ClientCredentialsGrantType }
        };

        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

        Token = new KeycloakServiceToken
        {
            AccessToken = responseToken!.AccessToken,
            Expires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn)
        };
    }

    private async Task UpdateServiceTokenIfNeeded()
    {
        if (IsTokenExpired())
            try
            {
                await TokenSemaphore.WaitAsync();
                await UpdateServiceToken();
            }
            finally
            {
                TokenSemaphore.Release();
            }
    }

    private static bool IsTokenExpired()
    {
        return Token == null ||
               Token.Expires <= DateTime.UtcNow.AddSeconds(TokenExpirationThresholdInSeconds);
    }
}