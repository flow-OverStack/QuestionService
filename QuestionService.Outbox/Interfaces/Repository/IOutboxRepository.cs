using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Interfaces.Repository;

public interface IOutboxRepository
{
    /// <summary>
    ///     Adds a message to the outbox.
    /// </summary>
    /// <param name="message">The outbox message to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a batch of unprocessed outbox messages.
    /// </summary>
    /// <param name="batchSize">The maximum number of messages to retrieve in the batch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of unprocessed outbox messages.</returns>
    Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks an outbox message as processed.
    /// </summary>
    /// <param name="messageId">The unique identifier of the outbox message to mark as processed.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task MarkAsProcessedAsync(long messageId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks an outbox message as failed.
    /// </summary>
    /// <param name="messageId">The unique identifier of the message to mark as failed.</param>
    /// <param name="errorMessage">The error message associated with the failure.</param>
    /// <param name="retryCount">The new retry count for the message.</param>
    /// <param name="nextRetryAt">The timestamp indicating when the message should next be retried.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MarkAsFailedAsync(long messageId, string errorMessage, int retryCount, DateTime nextRetryAt,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks the specified outbox message as dead.
    /// </summary>
    /// <param name="messageId">The unique identifier of the outbox message to mark as dead.</param>
    /// <param name="errorMessage">The error message associated with marking the message as dead.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task MarkAsDeadAsync(long messageId, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Resets the processed messages in the outbox that were processed before the specified date.
    /// </summary>
    /// <param name="olderThen">The threshold date to reset processed messages. Resets all processed messages if null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResetProcessedAsync(DateTime? olderThen = null, CancellationToken cancellationToken = default);
}