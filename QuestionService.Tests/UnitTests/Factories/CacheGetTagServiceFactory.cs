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

internal class CacheGetTagServiceFactory
{
    private readonly IGetTagService _cacheGetTagService;

    public readonly GetTagService InnerGetTagService =
        (GetTagService)new GetTagServiceFactory().GetService();

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public readonly IBaseCacheRepository<Tag, long> TagCacheRepository =
        new TagCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public CacheGetTagServiceFactory()
    {
        _cacheGetTagService = new CacheGetTagService(TagCacheRepository, InnerGetTagService,
            new OptionsWrapper<RedisSettings>(RedisSettings));
    }

    public IGetTagService GetService()
    {
        return _cacheGetTagService;
    }
}