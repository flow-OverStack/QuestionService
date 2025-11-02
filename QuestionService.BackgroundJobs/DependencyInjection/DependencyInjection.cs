using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.BackgroundJobs.Jobs;

namespace QuestionService.BackgroundJobs.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Sets up hangfire jobs
    /// </summary>
    /// <param name="app"></param>
    public static void SetupHangfireJobs(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
            {
                using var scope = app.Services.CreateAsyncScope();
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                recurringJobManager.AddOrUpdate<SyncViewsJob>("ViewsSynchronization",
                    job => job.RunAsync(CancellationToken.None), "*/10 * * * *"); // Every ten minutes
                recurringJobManager.AddOrUpdate<OutboxResetJob>("OutboxReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
            }
        );
    }
}