using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;
using QuestionService.Tests.FunctionalTests.Base.Exception;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class ExceptionTests : ExceptionBaseFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DownvoteQuestion_ShouldBe_Exception()
    {
        //Arrange
        const long questionId = 1;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/downvote/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpvoteQuestion_ShouldBe_Exception()
    {
        //Arrange
        const long questionId = 1;

        //Act
        var response = await HttpClient.PatchAsync($"/api/v1.0/question/upvote/{questionId}", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<VoteQuestionDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}