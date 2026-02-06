namespace QuestionService.Domain.Interfaces.Validation;

public interface ITagValidator
{
    public bool IsValid(string name, string? description, out IEnumerable<string> errorMessages);
}