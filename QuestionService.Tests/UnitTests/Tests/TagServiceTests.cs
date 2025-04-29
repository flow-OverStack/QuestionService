using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class TagServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddTag_ShouldBe_Success()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto("NewTag", "NewTagDescription");

        //Act
        var result = await tagService.AddTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddTag_ShouldBe_LengthOutOfRange()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var result = await tagService.AddTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.LengthOutOfRange, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddTag_ShouldBe_TagAlreadyExists()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto(".NET", "NewTagDescription");

        //Act
        var result = await tagService.AddTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateTag_ShouldBe_Success()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto(".NET", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateTag_ShouldBe_LengthOutOfRange()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.LengthOutOfRange, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateTag_ShouldBe_TagNotFound()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto("WrongTag", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteTag_ShouldBe_Success()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        const string tagName = "Python";

        //Act
        var result = await tagService.DeleteTagAsync(tagName);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteTag_ShouldBe_TagNotFound()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        const string tagName = "WrongTag";

        //Act
        var result = await tagService.DeleteTagAsync(tagName);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}