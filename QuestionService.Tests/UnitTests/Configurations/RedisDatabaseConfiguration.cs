using Moq;
using QuestionService.Tests.Configurations;
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
                var views = ViewConfiguration.GetViews();

                if (key == "view:keys") return views.Keys;
                if (key.ToString().StartsWith("view:question:")) return views.Values;

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
                var views = ViewConfiguration.GetEmptyValuesViews();

                if (key == "view:keys") return views.Keys;
                if (key.ToString().StartsWith("view:question:")) return views.Values;

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
                var views = ViewConfiguration.GetInvalidValuesViews();

                if (key == "view:keys") return views.Keys;
                if (key.ToString().StartsWith("view:question:")) return views.Values;

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
                var views = ViewConfiguration.GetInvalidKeysViews();

                if (key == "view:keys") return views.Keys;
                if (key.ToString().StartsWith("view:question:")) return views.Values;

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
                var views = ViewConfiguration.GetSpamViews();

                if (key == "view:keys") return views.Keys;
                if (key.ToString().StartsWith("view:question:")) return views.Values;

                throw new NotSupportedException(KeyNotSupportedMessage);
            });
        mockDatabase
            .Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        return mockDatabase.Object;
    }
}