using Microsoft.Extensions.Options;
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

    public readonly IGetTagService InnerGetTagService =
        new GetTagServiceFactory().GetService();

    public readonly ITagCacheRepository TagCacheRepository =
        new TagCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()));

    public CacheGetTagServiceFactory()
    {
        _cacheGetTagService = new CacheGetTagService(TagCacheRepository, InnerGetTagService);
    }

    public IGetTagService GetService()
    {
        return _cacheGetTagService;
    }
}