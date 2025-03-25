using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Interfaces.Repositories;

public interface IOutboxRepository
{
    /// <summary>
    ///     Adds  message to outbox
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task AddAsync(OutboxMessage message);

    /// <summary>
    ///     Gets messages that have not been processed yet
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    Task MarkAsProcessedAsync(Guid messageId);

    /// <summary>
    /// Marks
    /// </summary>
    /// <param name="messageId"></param>
    /// <returns></returns>
    Task MarkAsFailedAsync(Guid messageId, string errorMessage);
}