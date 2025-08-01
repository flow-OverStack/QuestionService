using Confluent.Kafka;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Events;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Messaging.Producers;
using QuestionService.Messaging.Settings;

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
                // Scope is not created because IOptions<KafkaSettings> is a singleton
                using var provider = services.BuildServiceProvider();
                var kafkaMainTopic = provider.GetRequiredService<IOptions<KafkaSettings>>().Value.MainEventsTopic;

                rider.AddProducer<BaseEvent>(kafkaMainTopic,
                    new ProducerConfig { Acks = Acks.All, EnableIdempotence = false });

                rider.UsingKafka((context, factoryConfigurator) =>
                {
                    var kafkaHost = context.GetRequiredService<IOptions<KafkaSettings>>().Value.Host;
                    factoryConfigurator.Host(kafkaHost);
                });
            });

            configurator.AddConfigureEndpointsCallback((context, _, cfg) =>
            {
                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(30)
                ));

                cfg.UseDelayedRedelivery(r => r.Intervals(
                    TimeSpan.FromMinutes(1),
                    TimeSpan.FromMinutes(5),
                    TimeSpan.FromMinutes(10),
                    TimeSpan.FromHours(1),
                    TimeSpan.FromHours(12),
                    TimeSpan.FromHours(24)
                ));

                cfg.UseInMemoryOutbox(context);
            });
        });
    }

    private static void InitProducers(this IServiceCollection services)
    {
        services.AddScoped<IBaseEventProducer, BaseEventProducer>();
    }
}