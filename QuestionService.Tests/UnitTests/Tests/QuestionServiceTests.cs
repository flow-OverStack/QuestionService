using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class QuestionServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskQuestion_ShouldBe_Success()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskQuestion_ShouldBe_LengthOutOfRange()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", []);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.LengthOutOfRange, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskQuestion_ShouldBe_UserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 0;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskQuestion_ShouldBe_TagsNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", ["WrongTag", "Java"]);

        //Act
        var result = await questionService.AskQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_Success()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_LengthOutOfRange()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", []);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.LengthOutOfRange, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_UserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 0;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(0, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_OperationForbidden()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 2;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task EditQuestion_ShouldBe_TagsNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        var dto = new EditQuestionDto(1, "NewQuestionTitle", "NewQuestionBodyNewQuestionBody", ["WrongTag", "Java"]);

        //Act
        var result = await questionService.EditQuestionAsync(initiatorId, dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TagsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteQuestion_ShouldBe_Success()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteQuestion_ShouldBe_UserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteQuestion_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteQuestion_ShouldBe_OperationForbidden()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.DeleteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.OperationForbidden, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_Success()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_Success_WithDownvoteGiven()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 4;
        const long questionId = 3;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_UserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_TooLowReputation()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 3;
        const long questionId = 1;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_VoteAlreadyGiven()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionService.UpvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_Success()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_Success_WithUpvoteGiven()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 2;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_UserNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 0;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_QuestionNotFound()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 0;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_TooLowReputation()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 2;
        const long questionId = 1;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.TooLowReputation, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_VoteAlreadyGiven()
    {
        //Arrange
        var questionService = new QuestionServiceFactory().GetService();
        const long initiatorId = 1;
        const long questionId = 3;

        //Act
        var result = await questionService.DownvoteQuestionAsync(initiatorId, questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteAlreadyGiven, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}