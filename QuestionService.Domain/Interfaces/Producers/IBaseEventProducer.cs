using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Interfaces.Producers;

public interface IBaseEventProducer
{
    Task ProduceAsync(long userId, BaseEventType eventType);
}