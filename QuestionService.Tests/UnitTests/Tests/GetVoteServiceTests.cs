using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetVoteServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByDtos_ShouldBe_Success()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(1, 1),
            new(2, 2),
            new(0, 0),
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByDtos_ShouldBe_VoteNotFound()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByDtos_ShouldBe_VotesNotFound()
    {
        //Arrange
        var dtos = new List<VoteDto>
        {
            new(0, 0),
            new(0, 0)
        };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetByDtosAsync(dtos);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsVotes_ShouldBe_Success()
    {
        //Arrange
        var questionIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetQuestionsVotesAsync(questionIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetQuestionsVotes_ShouldBe_VotesNotFound()
    {
        //Arrange
        var questionIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetQuestionsVotesAsync(questionIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersVotes_ShouldBe_Success()
    {
        //Arrange
        var userIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetUsersVotesAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersVotes_ShouldBe_VotesNotFound()
    {
        //Arrange
        var userIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        //Act
        var result = await getVoteService.GetUsersVotesAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetVoteTypesVotes_ShouldBe_Success()
    {
        // Arrange
        var voteTypeIds = new List<long> { 1, 2, 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        // Act
        var result = await getVoteService.GetVoteTypesVotesAsync(voteTypeIds);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetVoteTypesVotes_ShouldBe_VotesNotFound()
    {
        // Arrange
        var voteTypeIds = new List<long> { 0 };
        var getVoteService = new CacheGetVoteServiceFactory().GetService();

        // Act
        var result = await getVoteService.GetVoteTypesVotesAsync(voteTypeIds);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VotesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}