using QuestionService.Domain.Settings;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class BusinessRulesConfiguration
{
    public static BusinessRules GetBusinessRules() => new()
    {
        TitleMinLength = 10,
        BodyMinLength = 30,
        MinReputationToUpvote = 15,
        MinReputationToDownvote = 125,
        DownvoteReputationChange = -1,
        UpvoteReputationChange = 1,
        TagMaxLength = 35,
        TagDescriptionMaxLength = 400,
        UserViewSpamThreshold = 30,
        UserFingerprintLength = 64
    };
}