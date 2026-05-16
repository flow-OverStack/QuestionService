using QuestionService.Domain.Entities;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public GraphQlCursorPaginatedResponse<GraphQlQuestion> Questions { get; set; }
    public GraphQlCursorPaginatedResponse<Tag> Tags { get; set; }
    public GraphQlOffsetPaginatedResponse<Vote> QuestionVotes { get; set; }
    public GraphQlOffsetPaginatedResponse<View> QuestionViews { get; set; }

    public GraphQlOffsetPaginatedResponse<VoteType> QuestionVoteTypes { get; set; }
}