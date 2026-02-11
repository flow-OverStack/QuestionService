using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Domain.Dtos.Tag;

public record CreateTagDto(string Name, string? Description) : IValidatableTag;