using System.Net;
using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class ViewValidator : AbstractValidator<(string UserIp, string UserFingerprint)>, IViewValidator
{
    public ViewValidator()
    {
        RuleFor(x => x.UserIp)
            .NotEmpty().WithMessage(ErrorMessage.InvalidDataFormat)
            .Must(x => IPAddress.TryParse(x, out _)).WithMessage(ErrorMessage.InvalidDataFormat);

        RuleFor(x => x.UserFingerprint)
            .NotEmpty().WithMessage(ErrorMessage.InvalidDataFormat)
            .MaximumLength(EntityConstraints.UserFingerprintLength).WithMessage(ErrorMessage.InvalidDataFormat);
    }

    public bool IsValid(string userIp, string userFingerprint, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        var instance = (UserIp: userIp, UserFingerprint: userFingerprint);

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage).Distinct();

        return result.IsValid;
    }
}