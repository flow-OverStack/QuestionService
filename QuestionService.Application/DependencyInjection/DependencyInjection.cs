using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestionService.Application.Mappings;
using QuestionService.Application.Services;
using QuestionService.Application.Validators;
using QuestionService.Domain.Dtos.Request.Page;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(QuestionMapping));
        services.InitRedisCaching();
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IQuestionService, Services.QuestionService>();
        services.AddScoped<IViewService, ViewService>();
        services.AddScoped<IViewDatabaseService, ViewService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IGetQuestionService, GetQuestionService>();
        services.AddScoped<IGetVoteService, GetVoteService>();
        services.AddScoped<IGetTagService, GetTagService>();
        services.AddScoped<IGetViewService, GetViewService>();

        services.AddScoped<INullSafeValidator<OffsetPageDto>, OffsetPageDtoValidator>();
        services.AddScoped<INullSafeValidator<CursorPageDto>, CursorPageDtoValidator>();
    }

    private static void InitRedisCaching(this IServiceCollection services)
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
    }
}