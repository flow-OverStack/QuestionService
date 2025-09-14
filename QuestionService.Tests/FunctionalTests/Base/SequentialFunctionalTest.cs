using Microsoft.Extensions.DependencyInjection;
using QuestionService.Tests.FunctionalTests.Configurations;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base;

public class SequentialFunctionalTest(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory), IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        ResetDb(scope);
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