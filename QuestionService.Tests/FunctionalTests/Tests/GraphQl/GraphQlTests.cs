using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Application.Resources;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
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

        Assert.NotEmpty(result!.Data.Questions.Edges);
        Assert.NotEqual(0, result.Data.Questions.TotalCount);
        Assert.All(result.Data.Questions.Edges, x =>
        {
            Assert.NotNull(x.Cursor);
            Assert.NotNull(x.Node);
        });

        Assert.NotEmpty(result.Data.Tags.Edges);
        Assert.NotEqual(0, result.Data.Tags.TotalCount);
        Assert.All(result.Data.Tags.Edges, x =>
        {
            Assert.NotNull(x.Cursor);
            Assert.NotNull(x.Node);
        });

        Assert.NotEmpty(result.Data.Votes.Items);
        Assert.NotEqual(0, result.Data.Votes.TotalCount);

        Assert.NotEmpty(result.Data.Views.Items);
        Assert.NotEqual(0, result.Data.Views.TotalCount);

        Assert.NotEmpty(result.Data.VoteTypes.Items);
        Assert.NotEqual(0, result.Data.VoteTypes.TotalCount);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_InvalidPaginationError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestWithInvalidPaginationQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(4, result!.Errors.Count);
        Assert.All(result.Errors, x => Assert.StartsWith(ErrorMessage.InvalidPagination, x.Message));
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(2, 2, 1, 1, 1, 1) };

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
        Assert.NotNull(result.Data.VoteType);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Null()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllByIdsQuery(0, 0, 0, 0, 0, 0) };

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
        Assert.Null(result.Data.VoteType);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestWithWrongArgument_ShouldBe_Error()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestWithWrongArgument };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Single(result!.Errors);
        Assert.NotNull(result.Errors[0].Extensions.Code);
    }
}