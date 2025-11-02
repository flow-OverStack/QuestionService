using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Repositories;

public class OutboxRepository(IBaseRepository<OutboxMessage> outboxRepository) : IOutboxRepository
{
    public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
    {
        await outboxRepository.CreateAsync(message, cancellationToken);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedAsync(int batchSize,
        CancellationToken cancellationToken = default)
    {
        // Multiple outbox instances can process the same messages concurrently.
        // But consumers should be idempotent (check their EventId) to handle this case.

        var unprocessedMessages = await outboxRepository.GetAll()
            .Where(x => x.ProcessedAt == null)
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
        message.ProcessedAt = DateTime.UtcNow;
        message.ErrorMessage = null;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsFailedAsync(long messageId, string errorMessage, int retryCount, DateTime nextRetryAt,
        CancellationToken cancellationToken = default)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId, cancellationToken);
        message.ErrorMessage = errorMessage;
        message.RetryCount = retryCount;
        message.NextRetryAt = nextRetryAt;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }
}