using Microsoft.Extensions.DependencyInjection;
using QuestionService.Tests.FunctionalTests.Configurations;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base;

public class SequentialFunctionalTest : BaseFunctionalTest, IAsyncLifetime
{
    protected SequentialFunctionalTest(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    public Task InitializeAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        ResetDb(scope);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private static void ResetDb(IServiceScope scope)
    {
        scope.PrepPopulation();
    }
}