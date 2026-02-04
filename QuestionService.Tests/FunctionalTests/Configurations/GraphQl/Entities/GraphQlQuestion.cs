using QuestionService.Domain.Entities;

namespace QuestionService.Tests.FunctionalTests.Configurations.GraphQl.Entities;

internal class GraphQlQuestion : Question
{
    public int Reputation { get; set; }
    public int ViewCount { get; set; }
}