using QuestionService.Domain.Entities;
using QuestionService.Domain.Resources;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.ServiceFactories;
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
    public async Task GetAll_ShouldBe_TagsNotFound()
    {
        //Arrange
        var getTagService =
            new GetTagServiceFactory(tagRepository: MockRepositoriesGetters.GetEmptyMockRepository<Tag>().Object)
                .GetService();

        //Act
        var result = await getTagService.GetAllAsync();

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByName_ShouldBe_Success()
    {
        //Arrange
        const string tagName = ".NET";
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByNameAsync(tagName);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByName_ShouldBe_TagNotFound()
    {
        //Arrange
        const string tagName = "WrongTag";
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetByNameAsync(tagName);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionTags_ShouldBe_Success()
    {
        //Arrange
        const long questionId = 1;
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionTags(questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionTags_ShouldBe_QuestionNotFound()
    {
        //Arrange
        const long questionId = 0;
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionTags(questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionTags_ShouldBe_TagsNotFound()
    {
        //Arrange
        const long questionId = 4; // Question without tags
        var getTagService = new GetTagServiceFactory().GetService();

        //Act
        var result = await getTagService.GetQuestionTags(questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}