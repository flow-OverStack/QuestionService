using Microsoft.Extensions.Options;
using QuestionService.Application.Services;
using QuestionService.Cache.Helpers;
using QuestionService.Cache.Settings;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.Cache.Repositories;

public class TagCacheRepository : ITagCacheRepository
{
    private readonly IBaseCacheRepository<Tag, long> _repository;
    private readonly IGetTagService _tagInner;

    public TagCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetTagService tagInner)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<Tag, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetTagKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
        );
        _tagInner = tagInner;
    }

    public Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _tagInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionTagsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _tagInner.GetQuestionsTagsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}