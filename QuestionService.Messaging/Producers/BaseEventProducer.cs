using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Messaging.Events;
using QuestionService.Outbox.Interfaces.Service;

namespace QuestionService.Messaging.Producers;

public class BaseEventProducer(IOutboxService outboxService) : IBaseEventProducer
{
    public async Task ProduceAsync(long userId, BaseEventType eventType, BaseEventType? cancelsEventType = null,
        CancellationToken cancellationToken = default)
    {
        var baseEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType.ToString(),
            CancelsEvent = cancelsEventType?.ToString()
        };

        await outboxService.AddToOutboxAsync(baseEvent, cancellationToken);
    }
}