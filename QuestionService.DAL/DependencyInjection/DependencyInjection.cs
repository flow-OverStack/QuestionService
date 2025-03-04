using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.DAL.Interceptors;
using QuestionService.DAL.Repositories;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;

namespace QuestionService.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");
        services.AddSingleton<DateInterceptor>();
        services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

        services.InitRepositories();
    }

    /// <summary>
    ///     Migrates the database
    /// </summary>
    /// <param name="services"></param>
    public static async Task MigrateDatabaseAsync(this IServiceCollection services)
    {
        var dbContext = services.BuildServiceProvider().GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Question>, BaseRepository<Question>>();
        services.AddScoped<IBaseRepository<Tag>, BaseRepository<Tag>>();
        services.AddScoped<IBaseRepository<QuestionTag>, BaseRepository<QuestionTag>>();
        services.AddScoped<IBaseRepository<Vote>, BaseRepository<Vote>>();
    }
}