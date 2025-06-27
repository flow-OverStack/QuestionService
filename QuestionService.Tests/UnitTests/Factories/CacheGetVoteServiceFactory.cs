using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Settings;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

internal class CacheGetVoteServiceFactory
{
    private readonly IGetVoteService _cacheGetVoteService;

    public readonly GetVoteService InnerGetVoteService =
        (GetVoteService)new GetVoteServiceFactory().GetService();

    public readonly RedisSettings RedisSettings = RedisSettingsConfiguration.GetRedisSettingsConfiguration();

    public readonly IBaseCacheRepository<Vote, VoteDto> VoteCacheRepository =
        new VoteCacheRepository(new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()));

    public CacheGetVoteServiceFactory()
    {
        _cacheGetVoteService = new CacheGetVoteService(VoteCacheRepository, InnerGetVoteService,
            new OptionsWrapper<RedisSettings>(RedisSettings));
    }

    public IGetVoteService GetService()
    {
        return _cacheGetVoteService;
    }
}