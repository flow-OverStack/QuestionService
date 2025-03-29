using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetQuestionServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetById_ShouldBe_Success()
    {
        //Arrange
        const long questionId = 1;
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdAsync(questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetById_ShouldBe_QuestionNotFound()
    {
        //Arrange
        const long questionId = 0;
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdAsync(questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsWithTag_ShouldBe_Success()
    {
        //Arrange
        const string tagName = ".NET";
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTag(tagName);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsWithTag_ShouldBe_TagNotFound()
    {
        //Arrange
        const string tagName = "WrongTag";
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTag(tagName);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}