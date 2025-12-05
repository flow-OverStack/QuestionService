using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Service;

namespace QuestionService.Messaging.Producers;

public class BaseEventProducer(IOutboxService outboxService) : IBaseEventProducer
{
    public Task ProduceAsync(long userId, long questionId, BaseEventType eventType,
        CancellationToken cancellationToken = default)
    {
        var baseEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            UserId = userId,
            EntityId = questionId,
            EntityType = nameof(Question),
            EventType = eventType.ToString(),
        };

        return outboxService.AddToOutboxAsync(baseEvent, cancellationToken);
    }
}