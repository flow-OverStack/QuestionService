using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetTagServiceFactory
{
    private readonly IGetTagService _cacheGetTagService;

    public readonly GetTagService InnerGetTagService =
        (GetTagService)new GetTagServiceFactory().GetService();

    public readonly ITagCacheRepository TagCacheRepository =
        new TagCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetTagService)new GetTagServiceFactory().GetService());

    public CacheGetTagServiceFactory()
    {
        _cacheGetTagService = new CacheGetTagService(TagCacheRepository, InnerGetTagService);
    }

    public IGetTagService GetService()
    {
        return _cacheGetTagService;
    }
}