using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetViewServiceFactory
{
    private readonly IGetViewService _cacheGetViewService;

    public readonly GetViewService InnerGetViewService =
        (GetViewService)new GetViewServiceFactory().GetService();

    public readonly IViewCacheRepository ViewCacheRepository =
        new ViewCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetViewService)new GetViewServiceFactory().GetService());

    public CacheGetViewServiceFactory()
    {
        _cacheGetViewService = new CacheGetViewService(ViewCacheRepository, InnerGetViewService);
    }

    public IGetViewService GetService()
    {
        return _cacheGetViewService;
    }
}