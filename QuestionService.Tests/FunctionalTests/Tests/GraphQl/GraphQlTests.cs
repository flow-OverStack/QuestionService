using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Domain.Resources;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Questions);
        Assert.NotNull(result.Data.Tags);
        Assert.NotNull(result.Data.Votes);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIdsAndNames_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsAndNamesQuery(2, 2, 1, ".NET") };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdsAndNamesResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Question);
        Assert.NotNull(result.Data.Tag);
        Assert.NotNull(result.Data.Vote);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIdsAndNames_ShouldBe_NotFound()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsAndNamesQuery(0, 1, 0, "WrongTag") };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result.Errors, x => x.Message == ErrorMessage.QuestionNotFound);
        Assert.Contains(result.Errors, x => x.Message == ErrorMessage.VoteNotFound);
        Assert.Contains(result.Errors, x => x.Message == ErrorMessage.TagNotFound);
    }
}