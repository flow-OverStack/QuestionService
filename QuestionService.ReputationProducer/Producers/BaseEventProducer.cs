using QuestionService.Domain.Enums;
using QuestionService.Domain.Events;
using QuestionService.Domain.Interfaces.Producers;
using QuestionService.Outbox.Interfaces.Services;

namespace QuestionService.ReputationProducer.Producers;

public class BaseEventProducer(IOutboxService outboxService) : IBaseEventProducer
{
    public async Task ProduceAsync(long userId, BaseEventType eventType)
    {
        var baseEvent = new BaseEvent
        {
            EventId = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType.ToString()
        };

        await outboxService.AddToOutboxAsync(baseEvent);
    }
}