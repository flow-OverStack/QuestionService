using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class TagDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<TagDataLoader>();
        const long tagId = 1;

        //Act
        var result = await dataLoader.LoadAsync(tagId);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Null()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<TagDataLoader>();
        const long tagId = 0;

        //Act
        var result = await dataLoader.LoadAsync(tagId);

        //Assert
        Assert.Null(result);
    }
}