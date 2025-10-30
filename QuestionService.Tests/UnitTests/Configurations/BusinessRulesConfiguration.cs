using QuestionService.Application.Settings;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class BusinessRulesConfiguration
{
    public static BusinessRules GetBusinessRules() => new()
    {
        TitleMinLength = 10,
        BodyMinLength = 30,
        MinReputationToUpvote = 15,
        MinReputationToDownvote = 125,
        UserViewSpamThreshold = 30,
    };
}