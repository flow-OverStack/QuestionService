using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

internal class ViewServiceFactory
{
    private readonly IViewService _viewService;
    public readonly BusinessRules BusinessRules = BusinessRulesConfiguration.GetBusinessRules();

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IDatabase RedisDatabase = RedisDatabaseConfiguration.GetRedisDatabaseConfiguration();
    public readonly IBaseRepository<View> ViewRepository = MockRepositoriesGetters.GetMockViewRepository().Object;

    public ViewServiceFactory(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) RedisDatabase = redisDatabase;

        _viewService = new ViewService(RedisDatabase, QuestionRepository, ViewRepository,
            new OptionsWrapper<BusinessRules>(BusinessRules));
    }

    public IViewService GetService()
    {
        return _viewService;
    }
}