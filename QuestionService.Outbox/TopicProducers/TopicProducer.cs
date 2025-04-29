using MassTransit;
using QuestionService.Outbox.Interfaces.TopicProducer;

namespace QuestionService.Outbox.TopicProducers;

public class TopicProducer<T>(ITopicProducer<T> producer) : ITopicProducer where T : class
{
    public bool CanProduce(Type messageType) => messageType == typeof(T);

    public async Task ProduceAsync(object message, CancellationToken cancellationToken = default)
    {
        if (CanProduce(message.GetType()))
            await producer.Produce((T)message, cancellationToken);
        else
            throw new ArgumentException($"Cannot produce message of type {message.GetType()}.");
    }
}