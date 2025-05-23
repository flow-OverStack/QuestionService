using Hangfire;
using Microsoft.AspNetCore.Builder;
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
                RecurringJob.AddOrUpdate<SyncViewsJob>("ViewsSynchronization",
                    job => job.RunAsync(CancellationToken.None), "*/10 * * * *"); // Every ten minutes 
            }
        );
    }
}