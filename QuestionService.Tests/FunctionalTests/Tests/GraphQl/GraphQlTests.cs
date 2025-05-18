using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
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
        Assert.NotNull(result.Data.Views);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIdsAndNames_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(2, 2, 1, 1, 1) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdAndNameResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Question);
        Assert.NotNull(result.Data.Tag);
        Assert.NotNull(result.Data.Vote);
        Assert.NotNull(result.Data.View);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIdsAndName_ShouldBe_Null()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(0, 0, 0, 0, 0) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllByIdAndNameResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Question);
        Assert.Null(result.Data.Tag);
        Assert.Null(result.Data.Vote);
        Assert.Null(result.Data.View);
    }
}