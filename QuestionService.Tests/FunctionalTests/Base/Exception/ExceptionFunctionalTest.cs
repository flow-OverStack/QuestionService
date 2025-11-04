using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Exception;

public class ExceptionFunctionalTest : IClassFixture<ExceptionFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;
    protected readonly IServiceProvider ServiceProvider;

    protected ExceptionFunctionalTest(ExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
        ServiceProvider = factory.Services;
    }
}