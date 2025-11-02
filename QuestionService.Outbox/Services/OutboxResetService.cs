using QuestionService.Domain.Results;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Service;

namespace QuestionService.Outbox.Services;

public class OutboxResetService(IOutboxRepository outboxRepository) : IOutboxResetService
{
    private const int MaxProcessedMessageLifetimeInDays = 7;

    public async Task<BaseResult> ResetOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-MaxProcessedMessageLifetimeInDays);

        await outboxRepository.ResetProcessedAsync(thresholdDate, cancellationToken);

        return BaseResult.Success();
    }
}