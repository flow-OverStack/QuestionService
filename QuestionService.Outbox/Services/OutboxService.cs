using Newtonsoft.Json;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Services;

public class OutboxService(IOutboxRepository outboxRepository) : IOutboxService
{
    public async Task AddToOutboxAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).FullName ?? typeof(T).Name,
            Content = JsonConvert.SerializeObject(message)
        };

        await outboxRepository.AddAsync(outboxMessage, cancellationToken);
    }
}