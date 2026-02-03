using FluentValidation;
using Microsoft.Extensions.Options;
using QuestionService.Application.Settings;
using QuestionService.Domain.Dtos.Page;
using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Application.Validators;

public class OffsetPageDtoValidator : AbstractValidator<OffsetPageDto>, INullSafeValidator<OffsetPageDto>
{
    public OffsetPageDtoValidator(IOptions<ContentRules> contentRules)
    {
        var maxPageSize = contentRules.Value.MaxPageSize;

        RuleFor(x => x.Skip).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).NotNull().InclusiveBetween(0, maxPageSize);
    }

    public bool IsValid(OffsetPageDto? instance, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        if (instance == null) return false;

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage);

        return result.IsValid;
    }
}