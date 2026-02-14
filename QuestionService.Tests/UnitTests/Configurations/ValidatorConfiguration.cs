using FluentValidation;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class ValidatorConfiguration<T>
{
    public static IValidator<T> GetValidator(AbstractValidator<T> validator)
    {
        validator.RuleLevelCascadeMode = CascadeMode.Stop;
        return validator;
    }
}