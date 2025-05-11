using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using QuestionService.Tests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.FunctionalTests.Base.Exception.SyncViewsJob;

public class ExceptionSyncViewsJobFunctionalTestWebAppFactory : FunctionalTestWebAppFactory
{
    private static IMock<IDatabase> GetExceptionMockRedisDatabase()
    {
        var mock = new Mock<IDatabase>();

        mock.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(new TestException());
        mock.Setup(x => x.SetRemoveAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue[]>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(new TestException());
        mock.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(new TestException());

        return mock;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDatabase>();
            services.AddScoped<IDatabase>(_ =>
            {
                var exceptionRedisDatabase = GetExceptionMockRedisDatabase().Object;

                return exceptionRedisDatabase;
            });
        });
    }
}