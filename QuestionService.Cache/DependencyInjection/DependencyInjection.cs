using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Cache.Providers;
using QuestionService.Domain.Interfaces.Provider;
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
    }

    private static void InitProviders(this IServiceCollection services)
    {
        services.AddScoped<ICacheProvider, RedisCacheProvider>();
    }
}