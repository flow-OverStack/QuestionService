using QuestionService.Domain.Settings;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class RedisSettingsConfiguration
{
    public static RedisSettings GetRedisSettingsConfiguration()
    {
        return new RedisSettings
        {
            TimeToLiveInSeconds = 300
        };
    }
}