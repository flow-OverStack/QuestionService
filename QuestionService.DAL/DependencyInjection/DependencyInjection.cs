using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.DAL.Repositories;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Messages;

namespace QuestionService.DAL.DependencyInjection;

public static class DependencyInjection
{
    public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgresSQL");
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        services.InitRepositories();
    }

    /// <summary>
    ///     Migrates the database
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Question>, BaseRepository<Question>>();
        services.AddScoped<IBaseRepository<Tag>, BaseRepository<Tag>>();
        services.AddScoped<IBaseRepository<QuestionTag>, BaseRepository<QuestionTag>>();
        services.AddScoped<IBaseRepository<Vote>, BaseRepository<Vote>>();
        services.AddScoped<IBaseRepository<VoteType>, BaseRepository<VoteType>>();
        services.AddScoped<IBaseRepository<OutboxMessage>, BaseRepository<OutboxMessage>>();
        services.AddScoped<IBaseRepository<View>, BaseRepository<View>>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}