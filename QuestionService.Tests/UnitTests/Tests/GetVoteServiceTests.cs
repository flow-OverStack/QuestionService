using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Resources;
using QuestionService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetVoteServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_Success()
    {
        //Arrange
        const long questionId = 2;
        const long userId = 1;
        var dto = new GetVoteDto(questionId, userId);
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetByIdsAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_QuestionNotFound()
    {
        //Arrange
        const long questionId = 0;
        const long userId = 1;
        var dto = new GetVoteDto(questionId, userId);
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetByIdsAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_VoteNotFound()
    {
        //Arrange
        const long questionId = 2;
        const long userId = 3; //User has not voted the question
        var dto = new GetVoteDto(questionId, userId);
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetByIdsAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionVotes_ShouldBe_Success()
    {
        //Arrange
        const long questionId = 2;
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetQuestionVotesAsync(questionId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionVotes_ShouldBe_QuestionNotFound()
    {
        //Arrange
        const long questionId = 0;
        var getVoteService = new GetVoteServiceFactory().GetVoteService();

        //Act
        var result = await getVoteService.GetQuestionVotesAsync(questionId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.QuestionNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}