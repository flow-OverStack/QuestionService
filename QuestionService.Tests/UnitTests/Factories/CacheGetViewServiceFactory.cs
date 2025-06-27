using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetViewServiceFactory
{
    private readonly IGetViewService _cacheGetViewService;

    public readonly GetViewService InnerGetViewService =
        (GetViewService)new GetViewServiceFactory().GetService();

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public readonly IBaseCacheRepository<View, long> ViewCacheRepository =
        new ViewCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public CacheGetViewServiceFactory()
    {
        _cacheGetViewService = new CacheGetViewService(ViewCacheRepository, InnerGetViewService,
            new OptionsWrapper<RedisSettings>(RedisSettings));
    }

    public IGetViewService GetService()
    {
        return _cacheGetViewService;
    }
}