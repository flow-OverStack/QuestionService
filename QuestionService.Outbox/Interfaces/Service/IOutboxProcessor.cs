namespace QuestionService.Outbox.Interfaces.Service;

public interface IOutboxProcessor
{
    /// <summary>
    ///     Processes an outbox message
    /// </summary>
    /// <returns></returns>
    Task<int> ProcessOutboxMessagesAsync(int batchSize, CancellationToken cancellationToken = default);
}