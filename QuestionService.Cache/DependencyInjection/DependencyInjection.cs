using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Cache.DependencyInjection;

public static class DependencyInjection
{
    public static void AddCache(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>().Value;
            var configuration = new ConfigurationOptions
            {
                EndPoints = { { redisSettings.Host, redisSettings.Port } },
                Password = redisSettings.Password,
            };

            return ConnectionMultiplexer.Connect(configuration);
        });

        services.AddScoped<IDatabase>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        services.InitProviders();
        services.InitRepositories();
    }

    private static void InitProviders(this IServiceCollection services)
    {
        services.AddScoped<ICacheProvider, RedisCacheProvider>();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseCacheRepository<Question, long>, QuestionCacheRepository>();
        services.AddScoped<IBaseCacheRepository<Tag, long>, TagCacheRepository>();
        services.AddScoped<IBaseCacheRepository<View, long>, ViewCacheRepository>();
        services.AddScoped<IBaseCacheRepository<Vote, VoteDto>, VoteCacheRepository>();
    }
}