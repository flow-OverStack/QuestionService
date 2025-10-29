using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Interfaces.Producer;

public interface IBaseEventProducer
{
    Task ProduceAsync(long userId, BaseEventType eventType, BaseEventType? cancelsEventType = null,
        CancellationToken cancellationToken = default);
}