using QuestionService.Application.Settings;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class BusinessRulesConfiguration
{
    public static ContentRules GetBusinessRules() => new()
    {
        TitleMinLength = 10,
        BodyMinLength = 30,
        UserViewSpamThreshold = 30,
    };
}