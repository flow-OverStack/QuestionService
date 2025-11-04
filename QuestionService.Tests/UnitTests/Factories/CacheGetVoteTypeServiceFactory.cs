using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Application.Services.Cache;
using QuestionService.Cache.Providers;
using QuestionService.Cache.Repositories;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Tests.UnitTests.Configurations;

namespace QuestionService.Tests.UnitTests.Factories;

public class CacheGetVoteTypeServiceFactory
{
    private readonly IGetVoteTypeService _getVoteTypeService;

    public readonly GetVoteTypeService InnerGetVoteTypeService =
        (GetVoteTypeService)new GetVoteTypeServiceFactory().GetService();

    public readonly IVoteTypeCacheRepository VoteCacheRepository =
        new VoteTypeCacheRepository(
            new RedisCacheProvider(RedisDatabaseConfiguration.GetRedisDatabaseConfiguration()),
            Options.Create(RedisSettingsConfiguration.GetRedisSettingsConfiguration()),
            (GetVoteTypeService)new GetVoteTypeServiceFactory().GetService());

    public CacheGetVoteTypeServiceFactory()
    {
        _getVoteTypeService = new CacheGetVoteTypeService(VoteCacheRepository, InnerGetVoteTypeService);
    }

    public IGetVoteTypeService GetService()
    {
        return _getVoteTypeService;
    }
}