using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetViewServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getViewService = new GetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_Success()
    {
        //Arrange
        var viewIds = new List<long> { 1, 2, 0 };
        var getViewService = new GetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_ViewNotFound()
    {
        //Arrange
        var viewIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_ViewsNotFound()
    {
        //Arrange
        var viewIds = new List<long> { 0, 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetByIdsAsync(viewIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersViews_ShouldBe_Success()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getViewService = new GetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetUsersViewsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersViews_ShouldBe_ViewsNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetUsersViewsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsViews_ShouldBe_Success()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getViewService = new GetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetQuestionsViewsAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsViews_ShouldBe_ViewsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getViewService = new CacheGetViewServiceFactory().GetService();

        //Act
        var result = await getViewService.GetQuestionsViewsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ViewsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}