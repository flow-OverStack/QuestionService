using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Forbidden_When_ClaimsNotValid()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/question";
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser2", 2, []);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await HttpClient.PutAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Unauthorized()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/question";

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Forbidden()
    {
        //Arrange
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", 1, [
            new RoleDto { Name = "User" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        const string forbiddenUrl = "/api/v1.0/tag";

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}