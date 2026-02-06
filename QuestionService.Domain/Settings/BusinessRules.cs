namespace QuestionService.Domain.Settings;

public static class BusinessRules
{
    public const int TitleMinLength = 10;
    public const int TitleMaxLength = 150;

    public const int BodyMinLength = 30;
    public const int BodyMaxLength = 30000;

    public const int TagMaxLength = 35;
    public const int TagDescriptionMaxLength = 400;
    public const int MaxTagsCount = 5;

    public const int UserViewSpamThreshold = 30;
}