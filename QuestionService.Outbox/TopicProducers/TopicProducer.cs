using MassTransit;
using QuestionService.Outbox.Interfaces.TopicProducer;

namespace QuestionService.Outbox.TopicProducers;

public class TopicProducer<TEvent>(ITopicProducer<TEvent> producer) : ITopicProducer where TEvent : class
{
    public bool CanProduce(Type messageType)
    {
        return messageType == typeof(TEvent);
    }

    public Task ProduceAsync(object message, CancellationToken cancellationToken = default)
    {
        if (!CanProduce(message.GetType()))
            throw new ArgumentException($"Cannot produce message of type {message.GetType()}.");

        return producer.Produce((TEvent)message, cancellationToken);
    }
}