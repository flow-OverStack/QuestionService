using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using QuestionService.DAL;
using QuestionService.Domain.Settings;
using QuestionService.Grpc;
using QuestionService.Tests.FunctionalTests.Configurations;
using QuestionService.Tests.FunctionalTests.Configurations.TestServices.Grpc;
using QuestionService.Tests.FunctionalTests.Extensions;
using QuestionService.Tests.FunctionalTests.Helper;
using Testcontainers.PostgreSql;
using WireMock.Server;
using Xunit;

namespace QuestionService.Tests.FunctionalTests.Base;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _userServicePostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("user-service-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private WireMockServer _wireMockServer = null!;

    public async Task InitializeAsync()
    {
        await _userServicePostgreSql.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _userServicePostgreSql.StopAsync();
        _wireMockServer.StopServer();
        _wireMockServer.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            var userServiceConnectionString = _userServicePostgreSql.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(userServiceConnectionString));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            scope.PrepPopulation();

            _wireMockServer = _wireMockServer.StartIdentityServer();

            services.RemoveAll<IOptions<KeycloakSettings>>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Host = _wireMockServer.Url!;
                x.Realm = WireMockIdentityServerExtensions.RealmName;
                x.AdminToken = "TestAdminToken";
                x.Audience = TokenHelper.GetAudience();
                x.ClientId = "TestClientId";
            });

            services.RemoveAll<IOptions<GrpcHosts>>();
            services.Configure<GrpcHosts>(x => { x.UsersHost = "TestUsersHost"; });

            services.RemoveAll<UserService.UserServiceClient>();
            services.AddSingleton<UserService.UserServiceClient, GrpcTestUserService>();
        });
    }
}