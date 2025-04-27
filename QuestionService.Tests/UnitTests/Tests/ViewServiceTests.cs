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
    public async Task IncrementViews_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var dto = new IncrementViewsDto(0, 1, "1.0.0.1", "someFingerprint");
        var viewService = new ViewServiceFactory().GetService();

        //Act
        var result = await viewService.IncrementViewsAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task IncrementViews_ShouldBe_Exception()
    {
        //Arrange
        var dto = new IncrementViewsDto(1, 1, "1.0.0.1", "someFingerprint");
        var viewService = new ViewServiceFactory(RedisDatabaseConfiguration.GetExceptionRedisDatabaseConfiguration())
            .GetService();

        //Act
        var action = async () => await viewService.IncrementViewsAsync(dto);

        //Assert
        var exception = await Assert.ThrowsAsync<RedisException>(action);
        Assert.Equal("An exception occured while executing the redis command.", exception.Message);
    }
}