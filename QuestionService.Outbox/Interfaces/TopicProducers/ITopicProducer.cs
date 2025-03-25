namespace QuestionService.Outbox.Interfaces.TopicProducers;

public interface ITopicProducer
{
    /// <summary>
    ///     Checks if message of type messageType can be produced or not
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    bool CanProduce(Type messageType);

    /// <summary>
    ///     Produces the message 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Produce(object message, CancellationToken cancellationToken = default);
}