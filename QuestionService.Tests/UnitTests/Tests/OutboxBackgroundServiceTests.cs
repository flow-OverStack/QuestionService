using QuestionService.Tests.FunctionalTests.Configurations.TestServices;
using QuestionService.Tests.UnitTests.Configurations;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class OutboxBackgroundServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task ExecuteBackgroundJob_ShouldBe_NoException()
    {
        //Arrange
        var outboxService =
            new TestableOutboxBackgroundService(LoggerConfiguration.GetLogger(), null); //passing null for exception

        //Act
        await outboxService.ExecuteAsync();

        //Assert
        // If any exception is thrown, test will fail
        Assert.True(true);
    }
}