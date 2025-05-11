using QuestionService.Domain.Interfaces.Service;
using Serilog;

namespace QuestionService.BackgroundJobs.Jobs;

public class SyncViewsJob(IViewDatabaseService viewService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await viewService.SyncViewsToDatabaseAsync(cancellationToken);
            if (result.IsSuccess)
                logger.Information("Successfully synced {count} views to database", result.Data.SyncedViewsCount);
            else
                logger.Error("Failed to sync views to database: {message}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to sync views to database: {message}", e.Message);
        }
    }
}