using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetTagServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByNames_ShouldBe_Success()
    {
        //Arrange
        var tagNames = new List<string> { ".NET", "Java", "WrongTag" };
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByNamesAsync(tagNames);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByNames_ShouldBe_TagNotFound()
    {
        //Arrange
        var tagNames = new List<string> { "WrongTag" };
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByNamesAsync(tagNames);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByNames_ShouldBe_TagsNotFound()
    {
        //Arrange
        var tagNames = new List<string> { "WrongTag", "WrongTag" };
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByNamesAsync(tagNames);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsTags_ShouldBe_Success()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionsTags(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsTags_ShouldBe_TagsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionsTags(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}