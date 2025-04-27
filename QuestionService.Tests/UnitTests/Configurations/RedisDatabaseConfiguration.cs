using Moq;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Configurations;

internal static class RedisDatabaseConfiguration
{
    public static IDatabase GetRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        var trueRedisResult = RedisResult.Create(1, ResultType.Integer);


        mockDatabase
            .Setup(x => x.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]>(),
                It.IsAny<RedisValue[]>(),
                CommandFlags.None
            ))
            .ReturnsAsync(trueRedisResult);

        return mockDatabase.Object;
    }

    public static IDatabase GetExceptionRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        var trueRedisResult = RedisResult.Create(0, ResultType.Integer);


        mockDatabase
            .Setup(x => x.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]>(),
                It.IsAny<RedisValue[]>(),
                CommandFlags.None
            ))
            .ReturnsAsync(trueRedisResult);

        return mockDatabase.Object;
    }
}