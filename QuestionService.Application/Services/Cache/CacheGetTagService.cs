using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.Services.Cache;

public class CacheGetTagService(
    GetTagService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetTagService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Tag>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Tag>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToList();

        try
        {
            var keys = idsList.Select(RedisKeyHelper.GetTagKey);
            var tags = (await redisDatabase.GetJsonParsedAsync<Tag>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(tags.Select(x => x.Id)).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, tags);

            return CollectionResult<Tag>.Success(tags);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<Tag>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<Tag> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByIdsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<Tag>.Success(alreadyCachedList)
                    : result;

            var allTags = result.Data.UnionBy(alreadyCachedList, x => x.Id).ToList();

            var tagKeys = allTags.Select(x =>
                new KeyValuePair<string, Tag>(RedisKeyHelper.GetTagKey(x.Id), x));

            await redisDatabase.StringSetAsync(tagKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<Tag>.Success(allTags);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var idsList = questionIds.ToList();

        try
        {
            var questionTagKeys = idsList.Select(RedisKeyHelper.GetQuestionTagsKey);
            var questionTagStringIds =
                (await redisDatabase.SetsStringMembersAsync(questionTagKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var questionTagIds = questionTagStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var tagKeys = questionTagIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetTagKey)).Distinct();
            var tags = await redisDatabase.GetJsonParsedAsync<Tag>(tagKeys, cancellationToken);

            var questionTags = questionTagIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Tag>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => tags.FirstOrDefault(t => t.Id == v))
                            .Where(v => v != null)!))
                .ToList();

            var missingQuestionTags = idsList.Except(questionTagIds.Select(x => x.Key)).Distinct().ToList();
            var cachedQuestionTags = new List<KeyValuePair<long, IEnumerable<Tag>>>();
            foreach (var questionTagId in questionTagIds)
            {
                // Keys in questionTagIds are guaranteed to be in questionTags

                var actualQuestionTag = questionTags.First(x => x.Key == questionTagId.Key);

                if (questionTagId.Value.Except(actualQuestionTag.Value.Select(x => x.Id)).Any())
                    missingQuestionTags.Add(questionTagId.Key);
                else
                    cachedQuestionTags.Add(actualQuestionTag);
            }

            if (missingQuestionTags.Count > 0)
                return await GetFromInnerAndCacheAsync(missingQuestionTags, cachedQuestionTags);

            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(questionTags);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Tag>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetQuestionsTagsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(alreadyCachedList)
                    : result;

            var allQuestionTags = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var questionTagStringIds = allQuestionTags.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetQuestionTagsKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var tags = allQuestionTags.SelectMany(x => x.Value);
            var tagKeys = tags.Select(x => new KeyValuePair<string, Tag>(RedisKeyHelper.GetTagKey(x.Id), x));

            await redisDatabase.StringSetAsync(tagKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(questionTagStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(allQuestionTags);
        }
    }
}