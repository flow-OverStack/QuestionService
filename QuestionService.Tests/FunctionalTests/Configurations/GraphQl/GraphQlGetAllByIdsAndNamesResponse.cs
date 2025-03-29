using QuestionService.Domain.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllByIdsAndNamesResponse
{
    public GraphQlGetAllByIdsAndNamesData Data { get; set; }
}

internal class GraphQlGetAllByIdsAndNamesData
{
    public Question Question { get; set; }
    public Tag Tag { get; set; }
    public Vote Vote { get; set; }
}