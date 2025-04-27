using QuestionService.Domain.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllByIdAndNameResponse
{
    public GraphQlGetAllByIdAndNameData Data { get; set; }
}

internal class GraphQlGetAllByIdAndNameData
{
    public Question? Question { get; set; }
    public Tag? Tag { get; set; }
    public Vote? Vote { get; set; }
    public View? View { get; set; }
}