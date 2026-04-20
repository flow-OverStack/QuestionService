using Microsoft.Extensions.DependencyInjection;
using QuestionService.Outbox;
using Serilog;

namespace QuestionService.Tests.UnitTests.Configurations;

internal class TestableOutboxBackgroundService(ILogger logger, IServiceScopeFactory scopeFactory)
    : OutboxBackgroundService(logger, scopeFactory)
{
    public new Task ExecuteAsync(CancellationToken cancellationToken = default) => base.ExecuteAsync(cancellationToken);
}