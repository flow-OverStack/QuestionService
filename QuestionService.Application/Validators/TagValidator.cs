using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class TagValidator : AbstractValidator<IValidatableTag>
{
    public TagValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ErrorMessage.InvalidTagName)
            .MaximumLength(BusinessRules.TagMaxLength)
            .WithMessage(ErrorMessage.InvalidTagName);

        RuleFor(x => x.Description)
            .MaximumLength(BusinessRules.TagDescriptionMaxLength)
            .WithMessage(ErrorMessage.InvalidTagDescription);
    }
}