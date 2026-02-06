using FluentValidation;
using QuestionService.Application.Resources;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Validators;

public class QuestionValidator
    : AbstractValidator<(string Title, string Body, IEnumerable<string> Tags)>, IQuestionValidator
{
    public QuestionValidator(ITagValidator tagValidator)
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ErrorMessage.InvalidTitle)
            .MinimumLength(BusinessRules.TitleMinLength).WithMessage(ErrorMessage.InvalidTitle)
            .MaximumLength(BusinessRules.TitleMaxLength).WithMessage(ErrorMessage.InvalidTitle);

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage(ErrorMessage.InvalidBody)
            .MinimumLength(BusinessRules.BodyMinLength).WithMessage(ErrorMessage.InvalidBody)
            .MaximumLength(BusinessRules.BodyMaxLength).WithMessage(ErrorMessage.InvalidBody);

        RuleFor(x => x.Tags)
            .NotNull().WithMessage(ErrorMessage.InvalidTags)
            .Must(x =>
            {
                var tags = x.ToArray();
                return tags.Length is >= 1 and <= BusinessRules.MaxTagsCount;
            })
            .WithMessage(ErrorMessage.InvalidTags)
            .Must(x => x.All(t => tagValidator.IsValid(t, null, out _))) // Only validate tag names
            .WithMessage(ErrorMessage.InvalidTags);
    }

    public bool IsValid(string title, string body, IEnumerable<string> tags, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        var instance = (Title: title, Body: body, Tags: tags);

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage).Distinct();

        return result.IsValid;
    }
}