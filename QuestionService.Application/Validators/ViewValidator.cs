using System.Net;
using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class ViewValidator : AbstractValidator<IValidatableView>
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
}