using QuestionService.Domain.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl;

internal class GraphQlGetAllResponse
{
    public GraphQlGetAllData Data { get; set; }
}

internal class GraphQlGetAllData
{
    public List<Question> Questions { get; set; }
    public List<Tag> Tags { get; set; }
    public List<Vote> Votes { get; set; }
    public List<View> Views { get; set; }
}