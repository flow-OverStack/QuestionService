using Microsoft.Extensions.DependencyInjection;
using QuestionService.Application.Mappings;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Application.Validators;
using QuestionService.Domain.Dtos.Page;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Interfaces.Validation;

namespace QuestionService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(QuestionMapping));
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IQuestionService, Services.QuestionService>();
        services.AddScoped<IViewService, ViewService>();
        services.AddScoped<IViewDatabaseService, ViewService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<GetQuestionService>();
        services.AddScoped<IGetQuestionService, CacheGetQuestionService>();
        services.AddScoped<GetVoteService>();
        services.AddScoped<IGetVoteService, CacheGetVoteService>();
        services.AddScoped<GetTagService>();
        services.AddScoped<IGetTagService, CacheGetTagService>();
        services.AddScoped<GetViewService>();
        services.AddScoped<IGetViewService, CacheGetViewService>();

        services.AddScoped<INullSafeValidator<OffsetPageDto>, OffsetPageDtoValidator>();
        services.AddScoped<INullSafeValidator<CursorPageDto>, CursorPageDtoValidator>();
    }
}