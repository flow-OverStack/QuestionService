using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Tag;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

[Collection(nameof(TagServiceTests))]
public class TagServiceTests : SequentialFunctionalTest
{
    public TagServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var token = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", 1, [
            new RoleDto { Name = "Moderator" }
        ]);

        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task CreateTag_ShouldBe_Success()
    {
        //Arrange
        var dto = new CreateTagDto("NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task CreateTag_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new CreateTagDto("TooLongTagNameTooLongTagNameTooLongTagName", "NewTagDescription");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.LengthOutOfRange, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateTag_ShouldBe_Success()
    {
        //Arrange
        var dto = new TagDto(1, "NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateTag_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new TagDto(0, "NewTag", "NewTagDescription");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/tag", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteTag_ShouldBe_Success()
    {
        //Arrange
        const long tagId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/tag/{tagId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteTag_ShouldBe_BadRequest()
    {
        //Arrange
        const long tagId = 0;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/tag/{tagId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TagDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.TagNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}