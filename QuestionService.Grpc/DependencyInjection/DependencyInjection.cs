using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Grpc.Mapping;
using QuestionService.Grpc.Providers;

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
    }
}