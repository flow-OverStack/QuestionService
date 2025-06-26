using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetTagService(
    IBaseCacheRepository<Tag, long> cacheRepository,
    GetTagService inner,
    IOptions<RedisSettings> redisSettings) : IGetTagService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Tag>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Tag>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var tags = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (tags.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound),
                > 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound)
            };

        return CollectionResult<Tag>.Success(tags);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedTags = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            questionIds,
            CacheKeyHelper.GetQuestionTagsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetQuestionsTagsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedTags.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Failure(ErrorMessage.TagsNotFound,
                (int)ErrorCodes.TagsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(groupedTags);
    }
}