using StackExchange.Redis;

namespace QuestionService.Tests.Configurations;

internal static class ViewConfiguration
{
    public static (RedisValue[] Keys, RedisValue[] Values) GetViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ],
            [
                new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0_testFingerprint")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetEmptyValuesViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ], []
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetInvalidValuesViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3")
            ],
            [
                new RedisValue("WrongFormat"), new RedisValue("0")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetInvalidKeysViews()
    {
        return (
            [
                new RedisValue("view:question:wrongFormat")
            ],
            [
                new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0_testFingerprint")
            ]
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetSpamViews()
    {
        return (
            [
                new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                new RedisValue("view:question:3"), new RedisValue("view:question:4")
            ],
            [
                new RedisValue("1"), new RedisValue("0.0.0.0_testFingerprint1"),
                new RedisValue("0.0.0.0_testFingerprint2"), new RedisValue("0.0.0.0_testFingerprint3"),
                new RedisValue("0.0.0.0_testFingerprint4"), new RedisValue("0.0.0.0_testFingerprint5"),
                new RedisValue("0.0.0.0_testFingerprint6"), new RedisValue("0.0.0.0_testFingerprint7"),
                new RedisValue("0.0.0.0_testFingerprint8")
            ]
        );
    }
}