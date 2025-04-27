using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

public class ViewServiceFactory
{
    private readonly IViewService _viewService;

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IDatabase RedisDatabase = RedisDatabaseConfiguration.GetRedisDatabaseConfiguration();

    public ViewServiceFactory(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) RedisDatabase = redisDatabase;

        _viewService = new ViewService(RedisDatabase, QuestionRepository);
    }

    public IViewService GetService()
    {
        return _viewService;
    }
}