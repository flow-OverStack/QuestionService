namespace QuestionService.Domain.Interfaces.Validation;

public interface IViewValidator
{
    public bool IsValid(string userIp, string userFingerprint, out IEnumerable<string> errorMessages);
}