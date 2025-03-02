using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Interfaces.GraphQlClients;
using QuestionService.Domain.Settings;
using QuestionService.GraphQlClient.Auth;
using QuestionService.GraphQlClient.Clients.UserClient;
using QuestionService.GraphQlClient.Interfaces;

namespace QuestionService.GraphQlClient.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGraphQlClient(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddUserClient()
            .ConfigureHttpClient((provider, client) =>
            {
                var usersEndpoint = provider.GetRequiredService<IOptions<GraphQlEndpoints>>().Value
                    .UsersEndpoint;

                client.BaseAddress = new Uri(usersEndpoint);

                var graphQlAuthService = provider.GetRequiredService<IGraphQlAuthProvider>();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    graphQlAuthService.GetServiceTokenAsync().GetAwaiter().GetResult());
            });

        services.InitService();
    }

    private static void InitService(this IServiceCollection services)
    {
        services.AddScoped<IGraphQlClient<UserDto>, GraphQlUserClient>();
        services.AddScoped<IGraphQlAuthProvider, GraphQlAuthService>();
    }
}