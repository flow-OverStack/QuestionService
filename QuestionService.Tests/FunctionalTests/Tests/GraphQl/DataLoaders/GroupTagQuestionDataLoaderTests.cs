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
        const string tagName = ".NET";

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagName);

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
        const string tagName = "WrongTag";

        //Act
        var result = await dataLoader.LoadRequiredAsync(tagName);

        //Assert
        Assert.Empty(result);
    }
}