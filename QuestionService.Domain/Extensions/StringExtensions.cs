namespace QuestionService.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Checks if string has a minimum length or greater
    /// </summary>
    /// <param name="length"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasMinimumLength(this string input, int length)
    {
        return !string.IsNullOrEmpty(input)
               && input.Length >= length;
    }

    /// <summary>
    ///     Lowercases the first letter of input
    /// </summary>
    /// <param name="input"></param>
    /// <example>SomeInput -> someInput</example>
    /// <returns></returns>
    public static string LowercaseFirstLetter(this string input)
    {
        ArgumentException.ThrowIfNullOrEmpty(input);

        var output = char.ToLowerInvariant(input[0]) + input[1..];

        return output;
    }
}