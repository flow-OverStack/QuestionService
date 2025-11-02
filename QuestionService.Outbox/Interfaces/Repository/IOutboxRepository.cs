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
    Task MarkAsProcessedAsync(long messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="errorMessage"></param>
    /// <param name="retryCount"></param>
    /// <param name="nextRetryAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MarkAsFailedAsync(long messageId, string errorMessage, int retryCount, DateTime nextRetryAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Resets the processed messages in the outbox that were processed before the specified date.
    /// </summary>
    /// <param name="olderThen"></param>
    /// <param name="cancellationToken"></param>
    Task ResetProcessedAsync(DateTime? olderThen = null, CancellationToken cancellationToken = default);
}