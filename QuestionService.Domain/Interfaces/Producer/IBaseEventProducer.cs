using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Interfaces.Producer;

public interface IBaseEventProducer
{
    Task ProduceAsync(long authorId, long initiatorId, long questionId, BaseEventType eventType,
        CancellationToken cancellationToken = default);
}