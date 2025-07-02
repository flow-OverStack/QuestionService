using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Events;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Settings;
using QuestionService.Messaging.Producers;

namespace QuestionService.Messaging.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Adds message brokers with MassTransit
    /// </summary>
    /// <param name="services"></param>
    public static void AddMassTransitServices(this IServiceCollection services)
    {
        services.InitMassTransit();
        services.InitProducers();
    }

    private static void InitMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.UsingInMemory();

            configurator.AddRider(rider =>
            {
                // Scope is not created because IOptions<KafkaSettings> is singleton
                using var provider = services.BuildServiceProvider();
                var kafkaMainTopic = provider.GetRequiredService<IOptions<KafkaSettings>>().Value.MainEventsTopic;

                rider.AddProducer<BaseEvent>(kafkaMainTopic);

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaHost = context.GetRequiredService<IOptions<KafkaSettings>>().Value.Host;
                    factoryConfigurator.Host(kafkaHost);
                });
            });
        });
    }

    private static void InitProducers(this IServiceCollection services)
    {
        services.AddScoped<IBaseEventProducer, BaseEventProducer>();
    }
}