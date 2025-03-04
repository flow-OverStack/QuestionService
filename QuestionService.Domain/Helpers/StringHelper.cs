namespace QuestionService.Domain.Helpers;

public static class StringHelper
{
    public static bool HasMinimumLength(int length, string arg)
    {
        return string.IsNullOrEmpty(arg)
               && arg.Length >= length;
    }
}