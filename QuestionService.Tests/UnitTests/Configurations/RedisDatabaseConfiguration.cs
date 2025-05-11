using Moq;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class RedisDatabaseConfiguration
{
    private const string KeyNotSupportedMessage = "Key is not supported in tests.";

    public static IDatabase GetRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        var trueRedisResult = RedisResult.Create(1, ResultType.Integer);

        mockDatabase
            .Setup(x => x.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]>(),
                It.IsAny<RedisValue[]>(),
                It.IsAny<CommandFlags>()
            ))
            .ReturnsAsync(trueRedisResult);

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                if (key == "view:keys")
                    return
                    [
                        new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                        new RedisValue("view:question:3")
                    ];

                if (key.ToString().StartsWith("view:question:"))
                    return
                    [
                        new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0_testFingerprint")
                    ];

                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetFalseScriptRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        var falseRedisResult = RedisResult.Create(0, ResultType.Integer);

        mockDatabase
            .Setup(x => x.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]>(),
                It.IsAny<RedisValue[]>(),
                CommandFlags.None
            ))
            .ReturnsAsync(falseRedisResult);

        return mockDatabase.Object;
    }

    public static IDatabase GetEmptySetValuesDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                if (key == "view:keys")
                    return
                    [
                        new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                        new RedisValue("view:question:3")
                    ];

                if (key.ToString().StartsWith("view:question:")) return [];

                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetInvalidSetValuesDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                if (key == "view:keys")
                    return
                    [
                        new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                        new RedisValue("view:question:3")
                    ];

                if (key.ToString().StartsWith("view:question:"))
                    return
                    [
                        new RedisValue("WrongFormat")
                    ];


                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetInvalidSetKeysDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                if (key == "view:keys")
                    return [new RedisValue("view:question:wrongFormat")];

                if (key.ToString().StartsWith("view:question:"))
                    return
                    [
                        new RedisValue("1"), new RedisValue("2"), new RedisValue("0.0.0.0_testFingerprint")
                    ];

                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }

    public static IDatabase GetSpamDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
            {
                if (key == "view:keys")
                    return
                    [
                        new RedisValue("view:question:1"), new RedisValue("view:question:2"),
                        new RedisValue("view:question:3"), new RedisValue("view:question:4")
                    ];

                if (key.ToString().StartsWith("view:question:"))
                    return
                    [
                        new RedisValue("1"), new RedisValue("0.0.0.0_testFingerprint1"),
                        new RedisValue("0.0.0.0_testFingerprint2"), new RedisValue("0.0.0.0_testFingerprint3"),
                        new RedisValue("0.0.0.0_testFingerprint4"), new RedisValue("0.0.0.0_testFingerprint5"),
                        new RedisValue("0.0.0.0_testFingerprint6"), new RedisValue("0.0.0.0_testFingerprint7"),
                        new RedisValue("0.0.0.0_testFingerprint8")
                    ];

                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }
}