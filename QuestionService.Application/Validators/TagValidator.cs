using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class TagValidator : AbstractValidator<(string Name, string? Description)>, ITagValidator
{
    public TagValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(BusinessRules.TagMaxLength)
            .WithMessage(ErrorMessage.InvalidTagName);

        RuleFor(x => x.Description)
            .MaximumLength(BusinessRules.TagDescriptionMaxLength)
            .WithMessage(ErrorMessage.InvalidTagDescription);
    }

    public bool IsValid(string name, string? description, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        var instance = (Name: name, Description: description);

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage).Distinct();

        return result.IsValid;
    }
}