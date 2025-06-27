using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Providers;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

internal class ViewServiceFactory
{
    private readonly IViewDatabaseService _viewDatabaseService;
    private readonly IViewService _viewService;

    public readonly BusinessRules BusinessRules = BusinessRulesConfiguration.GetBusinessRules();

    public readonly ICacheProvider CacheProvider =
        new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration());

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;
    public readonly IBaseRepository<View> ViewRepository = MockRepositoriesGetters.GetMockViewRepository().Object;

    public ViewServiceFactory(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) CacheProvider = new RedisCacheProvider(redisDatabase);

        var service = new ViewService(CacheProvider, QuestionRepository, ViewRepository, UserProvider,
            new OptionsWrapper<BusinessRules>(BusinessRules));

        _viewService = service;
        _viewDatabaseService = service;
    }

    public IViewService GetService()
    {
        return _viewService;
    }

    public IViewDatabaseService GetDatabaseService()
    {
        return _viewDatabaseService;
    }
}