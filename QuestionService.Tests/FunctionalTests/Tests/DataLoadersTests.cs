using GreenDonut;
using HotChocolate;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Resources;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.Tests.FunctionalTests.Base;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class DataLoadersTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<QuestionDataLoader>();
        var questionIds = new List<long>
            { 1, 2 }; //When LoadRequiredAsync if some keys were not resolved the exception is thrown 

        //Act
        var questions = await dataLoader.LoadRequiredAsync(questionIds);

        //Assert
        Assert.Equal(questions.Count, questionIds.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_QuestionsNotFound()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<QuestionDataLoader>();
        var questionIds = new List<long> { 0 };

        //Act
        var action = async () => await dataLoader.LoadRequiredAsync(questionIds);

        //Assert
        var exception = await Assert.ThrowsAsync<GraphQLException>(action);
        Assert.Equal(ErrorMessage.QuestionsNotFound, exception.Message);
    }
}