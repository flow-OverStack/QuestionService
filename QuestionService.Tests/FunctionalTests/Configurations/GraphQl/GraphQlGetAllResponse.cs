using QuestionService.Domain.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlCursorPaginatedResponse<Question> Questions { get; set; }
    public GraphQlCursorPaginatedResponse<Tag> Tags { get; set; }
    public GraphQlOffsetPaginatedResponse<Vote> Votes { get; set; }
    public GraphQlOffsetPaginatedResponse<View> Views { get; set; }

    public GraphQlOffsetPaginatedResponse<VoteType> VoteTypes { get; set; }
}