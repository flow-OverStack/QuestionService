using Microsoft.Extensions.DependencyInjection;
using QuestionService.Outbox.Interfaces.Repositories;
using QuestionService.Outbox.Interfaces.Services;
using QuestionService.Outbox.Repositories;
using QuestionService.Outbox.Services;

namespace QuestionService.Outbox.DependencyInjection;

public static class DependencyInjection
{
    public static void AddOutbox(this IServiceCollection services)
    {
        services.InitServices();
        services.InitBackgroundServices();
    }

    private static void InitBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<OutboxBackgroundService>();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IOutboxService, OutboxService>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
    }
}