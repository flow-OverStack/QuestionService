using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GrpcServer.Mappings;
using QuestionService.GrpcServer.Services;

namespace QuestionService.GrpcServer.DependencyInjection;

public static class DependencyInjection
{
    public static void AddGrpcServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(QuestionMapping));
        services.AddGrpc();
    }

    public static void UseGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<GrpcQuestionService>();
    }
}