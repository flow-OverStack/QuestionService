using QuestionService.Domain.Dtos.Question;
using QuestionService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace QuestionService.Tests.UnitTests.Tests;

public class QuestionServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task AskQuestion_ShouldBe_Success()
    {
        //Arrange
        const long initiatorId = 1;
        var questionService = new QuestionServiceFactory().GetService();
        var dto = new AskQuestionDto("NewQuestionTitle", "NewQuestionBodyNewQuestionBody", [".NET"]);

        //Act
        var result = await questionService.AskQuestion(initiatorId, dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}