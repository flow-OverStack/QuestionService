using Microsoft.Extensions.DependencyInjection;
using QuestionService.Grpc.Mapping;

namespace QuestionService.Grpc.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcClients(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GrpcMapping));
    }
}