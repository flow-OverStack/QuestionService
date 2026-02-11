namespace QuestionService.Domain.Interfaces.Validation;

public interface IValidatableTag
{
    public string Name { get; }
    public string? Description { get; }
}