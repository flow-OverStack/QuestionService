using QuestionService.Application.Resources;
using QuestionService.Tests.UnitTests.Factories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class GetVoteTypeServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        //Act
        var result = await getVoteTypeService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_Success()
    {
        // Arrange
        var ids = new List<long> { 1, 2, 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_VoteTypeNotFound()
    {
        // Arrange
        var ids = new List<long> { 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypeNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_VoteTypesNotFound()
    {
        // Arrange
        var ids = new List<long> { 0, 0 };
        var getVoteTypeService = new CacheGetVoteTypeServiceFactory().GetService();

        // Act
        var result = await getVoteTypeService.GetByIdsAsync(ids);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.VoteTypesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}