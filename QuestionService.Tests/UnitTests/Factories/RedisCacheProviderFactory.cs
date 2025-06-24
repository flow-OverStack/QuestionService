using QuestionService.Cache.Providers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

public class RedisCacheProviderFactory
{
    private readonly ICacheProvider _cacheProvider;

    public readonly IDatabase RedisDatabase = RedisDatabaseConfiguration.GetRedisDatabaseConfiguration();

    public RedisCacheProviderFactory(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) RedisDatabase = redisDatabase;

        _cacheProvider = new RedisCacheProvider(RedisDatabase);
    }

    public ICacheProvider GetService()
    {
        return _cacheProvider;
    }
}