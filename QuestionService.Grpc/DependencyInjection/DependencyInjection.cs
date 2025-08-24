using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Grpc.Handlers;
using QuestionService.Grpc.Mappings;
using QuestionService.Grpc.Providers;
using QuestionService.Grpc.Settings;

namespace QuestionService.Grpc.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcClients(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GrpcMapping));
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IEntityProvider<UserDto>, UserProvider>();
        services.AddTransient<GrpcStatusMappingHandler>();

        var grpcBuilder = services.AddGrpcClient<UserService.UserServiceClient>((provider, opt) =>
        {
            var usersHost = provider.GetRequiredService<IOptions<GrpcHosts>>().Value.UsersHost;
            opt.Address = new Uri(usersHost);
        });

        grpcBuilder.AddStandardResilienceHandler();
        grpcBuilder.AddHttpMessageHandler<GrpcStatusMappingHandler>();
    }
}