using System.Text;

namespace QuestionService.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     Checks if string has a minimum length or greater
    /// </summary>
    /// <param name="length"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasMinLength(this string input, int length)
    {
        return !string.IsNullOrWhiteSpace(input)
               && input.Length >= length;
    }

    /// <summary>
    ///     Checks if string has a max length or less
    /// </summary>
    /// <param name="length"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasMaxLength(this string input, int length)
    {
        return !string.IsNullOrWhiteSpace(input)
               && input.Length <= length;
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

    /// <summary>
    /// Checks if string is encoded base64 string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsBase64(this string input)
    {
        return !string.IsNullOrWhiteSpace(input) &&
               Convert.TryFromBase64String(input, Encoding.UTF8.GetBytes(input), out _);
    }
}