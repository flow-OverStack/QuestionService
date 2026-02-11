namespace QuestionService.Domain.Interfaces.Validation;

public interface IValidatableView
{
    public string UserIp { get; }
    public string UserFingerprint { get; }
}