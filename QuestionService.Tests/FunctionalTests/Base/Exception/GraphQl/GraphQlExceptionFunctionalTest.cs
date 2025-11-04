using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class GraphQlExceptionFunctionalTest : IClassFixture<GraphQlExceptionFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected GraphQlExceptionFunctionalTest(GraphQlExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }
}