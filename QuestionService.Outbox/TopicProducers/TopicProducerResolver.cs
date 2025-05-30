using Microsoft.Extensions.DependencyInjection;
using QuestionService.Outbox.Interfaces.TopicProducer;

namespace QuestionService.Outbox.TopicProducers;

public class TopicProducerResolver(IServiceScopeFactory scopeFactory) : ITopicProducerResolver
{
    public ITopicProducer GetProducerForType(Type messageType)
    {
        using var scope = scopeFactory.CreateScope();
        var producers = scope.ServiceProvider.GetRequiredService<IEnumerable<ITopicProducer>>();

        return producers.FirstOrDefault(x => x.CanProduce(messageType)) ??
               throw new InvalidOperationException($"No producer found for type {messageType}.");
    }
}