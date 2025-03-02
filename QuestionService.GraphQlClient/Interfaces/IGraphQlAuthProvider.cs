namespace QuestionService.GraphQlClient.Interfaces;

internal interface IGraphQlAuthProvider
{
     Task<string> GetServiceTokenAsync();
}