namespace QuestionService.Domain.Helpers;

public static class StringHelper
{
    /// <summary>
    ///     Checks if string has a minimum length or greater
    /// </summary>
    /// <param name="length"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static bool HasMinimumLength(int length, string arg)
    {
        return !string.IsNullOrEmpty(arg)
               && arg.Length >= length;
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