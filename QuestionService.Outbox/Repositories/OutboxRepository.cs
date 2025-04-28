using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Outbox.Interfaces.Repositories;
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
        var unprocessedMessages =
            await outboxRepository.GetAll().Where(x => x.ProcessedAt == null).Take(batchSize)
                .ToListAsync(cancellationToken);
        return unprocessedMessages;
    }

    public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId, cancellationToken);
        message.ProcessedAt = DateTime.UtcNow;
        message.ErrorMessage = null;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsFailedAsync(Guid messageId, string errorMessage)
    {
        var message = await outboxRepository.GetAll().FirstAsync(x => x.Id == messageId);
        message.ErrorMessage = errorMessage;

        outboxRepository.Update(message);
        await outboxRepository.SaveChangesAsync();
    }
}