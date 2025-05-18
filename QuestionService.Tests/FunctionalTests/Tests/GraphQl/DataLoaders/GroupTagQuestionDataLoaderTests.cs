using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupTagQuestionDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagQuestionDataLoader>();
        const long tagId = 1;

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagId);

        //Assert
        Assert.Equal(3, result.Length); // Tag with name ".NET" has 3 questions
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoQuestions()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupTagQuestionDataLoader>();
        const long tagId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagId);

        //Assert
        Assert.Empty(result);
    }
}