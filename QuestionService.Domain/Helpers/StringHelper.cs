namespace QuestionService.Domain.Helpers;

public static class StringHelper
{
    public static bool AnyNullOrWhiteSpace(params string?[]? inputs)
    {
        ArgumentNullException.ThrowIfNull(inputs);

        return inputs.Any(string.IsNullOrWhiteSpace);
    }
}