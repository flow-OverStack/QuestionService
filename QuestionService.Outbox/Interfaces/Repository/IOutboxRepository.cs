using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Interfaces.Repository;

public interface IOutboxRepository
{
    /// <summary>
    ///     Adds  message to outbox
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets messages that have not been processed yet
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="errorMessage"></param>
    /// <param name="retryCount"></param>
    /// <param name="nextRetryAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MarkAsFailedAsync(Guid messageId, string errorMessage, int retryCount, DateTime nextRetryAt,
        CancellationToken cancellationToken = default);
}