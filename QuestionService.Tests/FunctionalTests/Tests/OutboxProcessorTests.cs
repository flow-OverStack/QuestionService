using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Events;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Tests.FunctionalTests.Base.Exception;
using Xunit;
using OutboxMessage = QuestionService.Outbox.Messages.OutboxMessage;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class OutboxProcessorTests(ExceptionFunctionalTestWebAppFactory factory) : ExceptionBaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ProcessOutboxMessages_ShouldBe_Ok()
    {
        //Arrange
        const long userId = 1;

        await using var scope = ServiceProvider.CreateAsyncScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();

        await outboxRepository.CreateAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Content = JsonConvert.SerializeObject(new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EventType = nameof(BaseEventType.QuestionUpvote),
                UserId = userId
            }),
            Type = typeof(BaseEvent).FullName ?? nameof(BaseEvent)
        });
        await outboxRepository.SaveChangesAsync();

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var unprocessedMessages =
            await outboxRepository.GetAll().Where(x => x.ProcessedAt == null).AsNoTracking().ToListAsync();
        Assert.True(unprocessedMessages.All(x => x.ErrorMessage != null));
    }
}