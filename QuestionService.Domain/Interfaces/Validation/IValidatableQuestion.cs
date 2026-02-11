namespace QuestionService.Domain.Interfaces.Validation;

public interface IValidatableQuestion
{
    public string Title { get; }
    public string Body { get; }
    public IEnumerable<string> TagNames { get; }
}