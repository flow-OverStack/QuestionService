using Microsoft.Extensions.DependencyInjection;
using QuestionService.Application.Mappings;
using QuestionService.Domain.Interfaces.Services;

namespace QuestionService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(GraphQlUserMapping));

        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IQuestionService, Services.QuestionService>();
    }
}