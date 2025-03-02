namespace QuestionService.GraphQlClient.HttpModels;

internal class KeycloakServiceToken
{
    public string AccessToken { get; set; }

    public DateTime Expires { get; set; }
}