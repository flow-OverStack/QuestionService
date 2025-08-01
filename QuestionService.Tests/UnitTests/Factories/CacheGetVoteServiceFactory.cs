using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetVoteServiceFactory
{
    private readonly IGetVoteService _cacheGetVoteService;

    public readonly GetVoteService InnerGetVoteService =
        (GetVoteService)new GetVoteServiceFactory().GetService();

    public readonly IVoteCacheRepository VoteCacheRepository =
        new VoteCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetVoteService)new GetVoteServiceFactory().GetService());

    public CacheGetVoteServiceFactory()
    {
        _cacheGetVoteService = new CacheGetVoteService(VoteCacheRepository, InnerGetVoteService);
    }

    public IGetVoteService GetService()
    {
        return _cacheGetVoteService;
    }
}