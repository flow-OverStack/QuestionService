using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Enums;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Repositories;

public class OutboxRepository(IBaseRepository<OutboxMessage> outboxRepository) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        message.Status = OutboxMessageStatus.Pending;
        await outboxRepository.CreateAsync(message, cancellationToken);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize,
        CancellationToken cancellationToken = default)
    {
        // Multiple outbox instances can process the same messages concurrently.
        // But consumers should be idempotent (check their EventId) to handle this case.

        var unprocessedMessages = await outboxRepository.GetAll()
            .Where(x => x.Status == OutboxMessageStatus.Pending || x.Status == OutboxMessageStatus.Failed)
            .Where(x => x.NextRetryAt == null || x.NextRetryAt <= DateTime.UtcNow)
            .OrderByDescending(x => x.RetryCount != 0)
            .ThenByDescending(x => x.NextRetryAt)
            .Take(batchSize)
            .ToArrayAsync(cancellationToken);

        return unprocessedMessages;
    }

    public async Task MarkAsProcessedAsync(long messageId, CancellationToken cancellationToken = default)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId, cancellationToken);

        message.Status = OutboxMessageStatus.Processed;
        message.ProcessedAt = DateTime.UtcNow;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsFailedAsync(long messageId, string errorMessage, int retryCount, DateTime nextRetryAt,
        CancellationToken cancellationToken = default)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId, cancellationToken);

        message.ErrorMessage = GetErrorMessage(message.ErrorMessage, errorMessage);
        message.RetryCount = retryCount;
        message.NextRetryAt = nextRetryAt;
        message.Status = OutboxMessageStatus.Failed;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsDeadAsync(long messageId, string errorMessage,
        CancellationToken cancellationToken = default)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId, cancellationToken);
        message.ErrorMessage = GetErrorMessage(message.ErrorMessage, errorMessage);
        message.Status = OutboxMessageStatus.Dead;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public Task ResetProcessedAsync(DateTime? olderThen = null, CancellationToken cancellationToken = default)
    {
        var query = outboxRepository.GetAll().Where(x => x.Status == OutboxMessageStatus.Processed);

        if (olderThen.HasValue) query = query.Where(x => x.ProcessedAt! < olderThen.Value);

        return query.ExecuteDeleteAsync(cancellationToken);
    }

    private static string GetErrorMessage(string? existingMessage, string newMessage)
    {
        if (string.IsNullOrEmpty(existingMessage)) return $"[{DateTime.UtcNow}] {newMessage}";
        return $"[{DateTime.UtcNow}] {newMessage}\n{existingMessage}";
    }
}