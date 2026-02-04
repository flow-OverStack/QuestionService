using QuestionService.Domain.Entities;
using QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Responses;

internal class GraphQlGetAllByIdAndNameResponse
{
    public GraphQlGetAllByIdAndNameData Data { get; set; }
}

internal class GraphQlGetAllByIdAndNameData
{
    public GraphQlQuestion? Question { get; set; }
    public Tag? Tag { get; set; }
    public Vote? Vote { get; set; }
    public View? View { get; set; }

    public VoteType? VoteType { get; set; }
}