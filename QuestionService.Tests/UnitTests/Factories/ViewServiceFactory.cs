using QuestionService.Application.Services;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

public class ViewServiceFactory
{
    private readonly IViewService _viewService;

    public readonly IDatabase RedisDatabase = RedisDatabaseConfiguration.GetRedisDatabaseConfiguration();

    public ViewServiceFactory()
    {
        _viewService = new ViewService(RedisDatabase);
    }

    public IViewService GetService()
    {
        return _viewService;
    }
}