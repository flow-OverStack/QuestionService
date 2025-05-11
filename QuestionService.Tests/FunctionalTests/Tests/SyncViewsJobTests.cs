using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.BackgroundJobs.Jobs;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Tests.FunctionalTests.Base;
using QuestionService.Tests.FunctionalTests.Helper;
using StackExchange.Redis;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Tests;

public class SyncViewsJobTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunReputationResetJob_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var redisDatabase = scope.ServiceProvider.GetRequiredService<IDatabase>();
        var syncViewsJob = ActivatorUtilities.CreateInstance<SyncViewsJob>(scope.ServiceProvider);
        var viewRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<View>>();

        await redisDatabase.InsertViews();

        //Act
        await syncViewsJob.RunAsync();

        //Assert
        var count = await viewRepository.GetAll().CountAsync();
        Assert.Equal(11, count); // Total 11 views including new ones
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RunReputationResetJob_ShouldBe_NoSyncesViews()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var redisDatabase = scope.ServiceProvider.GetRequiredService<IDatabase>();
        var syncViewsJob = ActivatorUtilities.CreateInstance<SyncViewsJob>(scope.ServiceProvider);
        var viewRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<View>>();

        await redisDatabase.InsertInvalidValuesViews();

        //Act
        await syncViewsJob.RunAsync();

        //Assert
        var count = await viewRepository.GetAll().CountAsync();
        Assert.Equal(4, count); // Total 4 views
    }
}