using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class TagServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateTag_ShouldBe_Success()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new CreateTagDto("NewTag", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateTag_ShouldBe_InvalidTagName()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new CreateTagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTagName, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateTag_ShouldBe_TagAlreadyExists()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new CreateTagDto(".NET", "NewTagDescription");

        //Act
        var result = await tagService.CreateTagAsync(dto);

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
        var dto = new TagDto(1, ".NET", "NewTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateTag_ShouldBe_InvalidTagDescription()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto(1, ".NET",
            "TooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescriptionTooLongTagDescription");

        //Act
        var result = await tagService.UpdateTagAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidTagDescription, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateTag_ShouldBe_TagNotFound()
    {
        //Arrange
        var tagService = new TagServiceFactory().GetService();
        var dto = new TagDto(0, "NewTag", "NewTagDescription");

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
        const long tagId = 3;

        //Act
        var result = await tagService.DeleteTagAsync(tagId);

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
        const long tagId = 0;

        //Act
        var result = await tagService.DeleteTagAsync(tagId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}