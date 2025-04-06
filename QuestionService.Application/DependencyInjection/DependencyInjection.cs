using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Application.Mappings;
using QuestionService.Application.Services;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(QuestionMapping));
        services.AddStackExchangeRedisCache(opt =>
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var redisSettings = scope.ServiceProvider.GetRequiredService<IOptions<RedisSettings>>().Value;

            opt.ConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { { redisSettings.Host, redisSettings.Port } },
                Password = redisSettings.Password,
            };
            opt.InstanceName = redisSettings.InstanceName;
        });

        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IQuestionService, Services.QuestionService>();
        services.AddScoped<IGetQuestionService, GetQuestionService>();
        services.AddScoped<IGetVoteService, GetVoteService>();
        services.AddScoped<IGetTagService, GetTagService>();
    }
}