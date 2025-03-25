using Newtonsoft.Json;
using QuestionService.Outbox.Interfaces.Repositories;
using QuestionService.Outbox.Interfaces.Services;
using QuestionService.Outbox.Messages;

namespace QuestionService.Outbox.Services;

public class OutboxService(IOutboxRepository outboxRepository) : IOutboxService
{
    public async Task AddToOutboxAsync<T>(T message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = typeof(T).FullName ?? typeof(T).Name,
            Content = JsonConvert.SerializeObject(message)
        };

        await outboxRepository.AddAsync(outboxMessage);
    }
}