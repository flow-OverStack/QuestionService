using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestionService.Outbox.Interfaces.Services;
using Serilog;

namespace QuestionService.Outbox;

public class OutboxBackgroundService(ILogger logger, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    private const int OutboxProcessorFrequencyInSeconds = 10;
    private const int BatchSize = 50;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.Information("{ServiceName} is running.", nameof(OutboxBackgroundService));
            while (!stoppingToken.IsCancellationRequested)
            {
                var processor = scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IOutboxProcessor>();
                var processed = await processor.ProcessOutboxMessagesAsync(BatchSize, stoppingToken);

                if (processed > 0)
                    logger.Information("Processed {ProcessedMessagesCount} messages", processed);

                await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequencyInSeconds), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.Information("{ServiceName} is canceled.", nameof(OutboxBackgroundService));
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occured while running {ServiceName}: {ErrorMessage}",
                nameof(OutboxBackgroundService), e.Message);
        }
    }
}