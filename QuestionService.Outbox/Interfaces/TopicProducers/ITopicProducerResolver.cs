namespace QuestionService.Outbox.Interfaces.TopicProducers;

public interface ITopicProducerResolver
{
    ITopicProducer GetProducerForType(Type messageType);
}