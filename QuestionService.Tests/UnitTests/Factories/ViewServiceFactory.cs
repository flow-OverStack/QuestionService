using QuestionService.Application.Services;
using QuestionService.Application.Validators;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.Configurations;
using QuestionService.Tests.UnitTests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.UnitTests.Factories;

internal class ViewServiceFactory
{
    private readonly IViewDatabaseService _viewDatabaseService;
    private readonly IViewService _viewService;

    public readonly IViewCacheSyncRepository CacheRepository =
        new ViewCacheSyncRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public readonly IBaseRepository<Question> QuestionRepository =
        MockRepositoriesGetters.GetMockQuestionRepository().Object;

    public readonly IEntityProvider<UserDto> UserProvider = MockEntityProvidersGetters.GetMockUserProvider().Object;
    public readonly IBaseRepository<View> ViewRepository = MockRepositoriesGetters.GetMockViewRepository().Object;

    public ViewServiceFactory(IDatabase? redisDatabase = null)
    {
        if (redisDatabase != null) CacheRepository = new ViewCacheSyncRepository(new RedisCacheProvider(redisDatabase));

        var service = new ViewService(CacheRepository, QuestionRepository, ViewRepository, UserProvider,
            new ViewValidator());

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