using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.Configurations;
using QuestionService.Tests.UnitTests.Factories;
using StackExchange.Redis;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class ViewServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncrementViews_ShouldBe_Success()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, null, "0.0.0.0", "someFingerprint");
        var viewService = new ViewServiceFactory().GetService();

        //Act
        var result = await viewService.IncrementViewsAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncrementViews_ShouldBe_InvalidDataFormat()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, null, "WrongIp", "someFingerprint");
        var viewService = new ViewServiceFactory().GetService();

        //Act
        var result = await viewService.IncrementViewsAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidDataFormat, result.ErrorMessage);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncrementViews_ShouldBe_Exception()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, 1, "1.0.0.1", "someFingerprint");
        var viewService =
            new ViewServiceFactory(RedisDatabaseConfiguration.GetFalseResponseRedisDatabaseConfiguration())
                .GetService();

        //Act
        var action = async () => await viewService.IncrementViewsAsync(dto);

        //Assert
        var exception = await Assert.ThrowsAsync<RedisException>(action);
        Assert.Equal("An exception occurred while executing the Redis command.", exception.Message);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncViewsToDatabase_ShouldBe_Success()
    {
        //Arrange
        var viewService = new ViewServiceFactory().GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(7, result.Data.SyncedViewsCount); // There are 7 new views in total
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncViewsToDatabase_ShouldBe_NoSyncedViews()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetEmptySetValuesDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncViewsToDatabase_ShouldBe_NoSyncedViews_When_KeysInvalid()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetInvalidSetKeysDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncViewsToDatabase_ShouldBe_NoSyncedViews_When_ValuesInvalid()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetInvalidSetValuesDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.Equal(0, result.Data.SyncedViewsCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncViewsToDatabase_ShouldBe_SpamFiltered()
    {
        //Arrange
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetSpamDatabaseConfiguration())
            .GetDatabaseService();

        //Act
        var result = await viewService.SyncViewsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.SyncedViewsCount); // Total 2 new views, others filtered or already exist
    }
}