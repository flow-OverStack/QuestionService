using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Events;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Tests.FunctionalTests.Base.Exception;
using Xunit;
using OutboxMessage = QuestionService.Outbox.Messages.OutboxMessage;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class OutboxProcessorTests(ExceptionFunctionalTestWebAppFactory factory) : ExceptionFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ProcessOutboxMessages_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();
        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

        await outboxService.AddToOutboxAsync(new BaseEvent
        {
            EventId = Guid.NewGuid(),
            EventType = nameof(BaseEventType.QuestionUpvote),
            UserId = userId
        });

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var unprocessedMessages =
            await outboxRepository.GetAll().Where(x => x.ProcessedAt == null).AsNoTracking().ToListAsync();
        Assert.True(unprocessedMessages.All(x => x.ErrorMessage != null));
    }
}