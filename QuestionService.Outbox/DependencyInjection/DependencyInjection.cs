using Microsoft.Extensions.DependencyInjection;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Interfaces.TopicProducer;
using QuestionService.Outbox.Repositories;
using QuestionService.Outbox.Services;
using QuestionService.Outbox.TopicProducers;

namespace QuestionService.Outbox.DependencyInjection;

public static class DependencyInjection
{
    public static void AddOutbox(this IServiceCollection services)
    {
        services.InitServices();
        services.InitBackgroundServices();
        services.InitTopicProducers();
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

    private static void InitTopicProducers(this IServiceCollection services)
    {
        services.AddSingleton<ITopicProducerResolver, TopicProducerResolver>();
        services.AddScoped<ITopicProducer, TopicProducer<BaseEvent>>();
    }
}