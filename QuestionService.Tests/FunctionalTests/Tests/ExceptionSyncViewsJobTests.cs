using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.BackgroundJobs.Jobs;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Tests.FunctionalTests.Base.Exception.SyncViewsJob;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class ExceptionSyncViewsJobTests(ExceptionSyncViewsJobFunctionalTestWebAppFactory factory)
    : ExceptionSyncViewsJobFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task SyncViews_ShouldBe_NoException()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var syncViewsJob = ActivatorUtilities.CreateInstance<SyncViewsJob>(scope.ServiceProvider);
        var viewRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<View>>();

        //Act
        await syncViewsJob.RunAsync();

        //Assert
        var count = await viewRepository.GetAll().CountAsync();
        Assert.Equal(4, count); // Total 4 views
    }
}