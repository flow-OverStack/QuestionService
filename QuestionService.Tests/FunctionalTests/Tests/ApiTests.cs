using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task EditQuestion_ShouldBe_Forbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser2", 2, [
            new RoleDto { Name = "User" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new EditQuestionDto(1, "NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task EditQuestion_ShouldBe_Forbidden_When_ClaimsNotValid()
    {
        //Arrange
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser2", 2, []);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new EditQuestionDto(1, "NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task EditQuestion_ShouldBe_Unauthorized()
    {
        //Arrange
        HttpClient.DefaultRequestHeaders.Authorization = null;

        var dto = new EditQuestionDto(1, "NewQuestion", "NewQuestionNewQuestionNewQuestion", [".NET"]);

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/question", dto);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestWrongUrl_ShouldBe_NotFound()
    {
        //Arrange
        const string wrongUrl = "wrongUrl";

        //Act
        var response = await HttpClient.GetAsync(wrongUrl);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Contains($"{(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}", body);
    }
}