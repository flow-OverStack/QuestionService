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

internal class CacheGetQuestionServiceFactory
{
    private readonly IGetQuestionService _cacheGetQuestionService;

    public readonly GetQuestionService InnerGetQuestionService =
        (GetQuestionService)new GetQuestionServiceFactory().GetService();

    public readonly IBaseCacheRepository<Question, long> QuestionCacheRepository =
        new QuestionCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public CacheGetQuestionServiceFactory()
    {
        _cacheGetQuestionService = new CacheGetQuestionService(QuestionCacheRepository, InnerGetQuestionService,
            Options.Create(RedisSettings));
    }

    public IGetQuestionService GetService()
    {
        return _cacheGetQuestionService;
    }
}