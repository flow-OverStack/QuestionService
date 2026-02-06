namespace QuestionService.Domain.Interfaces.Validation;

public interface IQuestionValidator
{
    public bool IsValid(string title, string body, IEnumerable<string> tags, out IEnumerable<string> errorMessages);
}