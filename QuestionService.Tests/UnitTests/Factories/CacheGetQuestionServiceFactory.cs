using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetQuestionServiceFactory
{
    private readonly IGetQuestionService _cacheGetQuestionService;

    public readonly GetQuestionService InnerGetQuestionService =
        (GetQuestionService)new GetQuestionServiceFactory().GetService();

    public readonly IQuestionCacheRepository QuestionCacheRepository =
        new QuestionCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetQuestionService)new GetQuestionServiceFactory().GetService());

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public CacheGetQuestionServiceFactory()
    {
        _cacheGetQuestionService = new CacheGetQuestionService(QuestionCacheRepository, InnerGetQuestionService);
    }

    public IGetQuestionService GetService()
    {
        return _cacheGetQuestionService;
    }
}