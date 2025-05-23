using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Events;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Outbox.Messages;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class OutboxBackgroundServiceTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task ExecuteBackgroundJob_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;

        using var scope = ServiceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OutboxMessage>>();

        await outboxRepository.CreateAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Content = JsonConvert.SerializeObject(new BaseEvent
            {
                EventId = Guid.NewGuid(),
                EventType = BaseEventType.QuestionUpvote.ToString(),
                UserId = userId
            }),
            Type = typeof(BaseEvent).FullName ?? nameof(BaseEvent)
        });
        await outboxRepository.SaveChangesAsync();

        //Act
        await Task.Delay(TimeSpan.FromSeconds(20)); //Waiting for OutboxBackgroundService to execute the job

        //Assert
        var outboxMessages = await outboxRepository.GetAll().AsNoTracking().ToListAsync();
        Assert.True(outboxMessages.All(x => x.ProcessedAt != null));
    }
}