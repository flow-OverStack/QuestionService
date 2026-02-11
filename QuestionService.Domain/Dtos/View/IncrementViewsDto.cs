using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Domain.Dtos.View;

public record IncrementViewsDto(long QuestionId, long? UserId, string UserIp, string UserFingerprint)
    : IValidatableView;