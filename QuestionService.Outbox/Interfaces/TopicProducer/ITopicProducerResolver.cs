namespace QuestionService.Outbox.Interfaces.TopicProducer;

public interface ITopicProducerResolver
{
    ITopicProducer GetProducerForType(Type messageType);
}