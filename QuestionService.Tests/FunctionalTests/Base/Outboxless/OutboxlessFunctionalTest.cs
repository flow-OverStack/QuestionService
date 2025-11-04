using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Outboxless;

public class OutboxlessFunctionalTest : IClassFixture<OutboxlessFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected OutboxlessFunctionalTest(OutboxlessFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}