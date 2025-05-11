using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.SyncViewsJob;

public class ExceptionSyncViewsJobFunctionalTest : IClassFixture<ExceptionSyncViewsJobFunctionalTestWebAppFactory>
{
    protected readonly IServiceProvider ServiceProvider;

    protected ExceptionSyncViewsJobFunctionalTest(ExceptionSyncViewsJobFunctionalTestWebAppFactory factory)
    {
        ServiceProvider = factory.Services;
    }
}