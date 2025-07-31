using QuestionService.Domain.Settings;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class EntityRulesConfiguration
{
    public static EntityRules GetEntityRules()
    {
        return new EntityRules
        {
            TagMaxLength = 35,
            TagDescriptionMaxLength = 400,
            UserFingerprintLength = 64
        };
    }
}