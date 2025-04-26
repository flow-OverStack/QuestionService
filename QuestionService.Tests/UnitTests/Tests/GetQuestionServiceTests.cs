using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.Factories;
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
    public async Task GetByIds_ShouldBe_Success()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_QuestionsNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0, 0 };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetByIdsAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsWithTags_ShouldBe_Success()
    {
        //Arrange
        var tagNames = new List<string> { ".NET", "Java", "WrongTag" };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTags(tagNames);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsWithTags_ShouldBe_QuestionsNotFound()
    {
        //Arrange
        var tagNames = new List<string> { "WrongTag", "WrongTag" };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetQuestionsWithTags(tagNames);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersQuestions_ShouldBe_Success()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetUsersQuestions(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersQuestions_ShouldBe_QuestionsNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0, 0 };
        var getQuestionService = new GetQuestionServiceFactory().GetService();

        //Act
        var result = await getQuestionService.GetUsersQuestions(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}