using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.Services.Cache;

public class CacheGetQuestionService(
    GetQuestionService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetQuestionService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToList();

        try
        {
            var keys = idsList.Select(RedisKeyHelper.GetQuestionKey);
            var questions = (await redisDatabase.GetJsonParsedAsync<Question>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(questions.Select(x => x.Id)).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, questions);

            return CollectionResult<Question>.Success(questions);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<Question>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<Question> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByIdsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<Question>.Success(alreadyCachedList)
                    : result;

            var allQuestions = result.Data.UnionBy(alreadyCachedList, x => x.Id).ToList();

            var keyQuestions = allQuestions.Select(x =>
                new KeyValuePair<string, Question>(RedisKeyHelper.GetQuestionKey(x.Id), x));

            await redisDatabase.StringSetAsync(keyQuestions, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<Question>.Success(allQuestions);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds, CancellationToken cancellationToken = default)
    {
        var idsList = tagIds.ToList();

        try
        {
            var tagQuestionKeys = idsList.Select(RedisKeyHelper.GetTagQuestionsKey);
            var tagsQuestionStringIds =
                (await redisDatabase.SetsStringMembersAsync(tagQuestionKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var tagQuestionIds = tagsQuestionStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var questionKeys = tagQuestionIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetQuestionKey)).Distinct();
            var questions = await redisDatabase.GetJsonParsedAsync<Question>(questionKeys, cancellationToken);

            var tagQuestions = tagQuestionIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Question>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => questions.FirstOrDefault(q => q.Id == v))
                            .Where(v => v != null)!))
                .ToList();

            var missingTagQuestions = idsList.Except(tagQuestionIds.Select(x => x.Key)).Distinct().ToList();
            var cachedTagQuestions = new List<KeyValuePair<long, IEnumerable<Question>>>();
            foreach (var tagQuestionId in tagQuestionIds)
            {
                // Keys in tagQuestionIds are guaranteed to be in tagQuestions

                var actualTagQuestion = tagQuestions.First(x => x.Key == tagQuestionId.Key);

                if (tagQuestionId.Value.Except(actualTagQuestion.Value.Select(x => x.Id)).Any())
                    missingTagQuestions.Add(tagQuestionId.Key);
                else
                    cachedTagQuestions.Add(actualTagQuestion);
            }

            if (missingTagQuestions.Count > 0)
                return await GetFromInnerAndCacheAsync(missingTagQuestions, cachedTagQuestions);

            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(tagQuestions);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Question>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetQuestionsWithTagsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(alreadyCachedList)
                    : result;

            var allTagQuestions = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var tagQuestionStringIds = allTagQuestions.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetTagQuestionsKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var questions = allTagQuestions.SelectMany(x => x.Value);
            var questionKeys = questions.Select(x =>
                new KeyValuePair<string, Question>(RedisKeyHelper.GetQuestionKey(x.Id), x));

            await redisDatabase.StringSetAsync(questionKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(tagQuestionStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(allTagQuestions);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var idsList = userIds.ToList();

        try
        {
            var userQuestionKeys = idsList.Select(RedisKeyHelper.GetUserQuestionsKey);
            var userQuestionStringIds =
                (await redisDatabase.SetsStringMembersAsync(userQuestionKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var userQuestionIds = userQuestionStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var questionKeys = userQuestionIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetQuestionKey))
                .Distinct();
            var questions = await redisDatabase.GetJsonParsedAsync<Question>(questionKeys, cancellationToken);

            var userQuestions = userQuestionIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<Question>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => questions.FirstOrDefault(q => q.Id == v))
                            .Where(v => v != null)!))
                .ToList();

            var missingUserQuestions = idsList.Except(userQuestionIds.Select(x => x.Key)).Distinct().ToList();
            var cachedUserQuestions = new List<KeyValuePair<long, IEnumerable<Question>>>();
            foreach (var userQuestionId in userQuestionIds)
            {
                // Keys in userQuestionIds are guaranteed to be in userQuestions

                var actualUserQuestion = userQuestions.First(x => x.Key == userQuestionId.Key);

                if (userQuestionId.Value.Except(actualUserQuestion.Value.Select(x => x.Id)).Any())
                    missingUserQuestions.Add(userQuestionId.Key);
                else
                    cachedUserQuestions.Add(actualUserQuestion);
            }

            if (missingUserQuestions.Count > 0)
                return await GetFromInnerAndCacheAsync(missingUserQuestions, cachedUserQuestions);

            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(userQuestions);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<Question>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetUsersQuestionsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(alreadyCachedList)
                    : result;

            var allUserQuestions = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var userQuestionStringIds = allUserQuestions.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetUserQuestionsKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var questions = allUserQuestions.SelectMany(x => x.Value);
            var questionKeys = questions.Select(x =>
                new KeyValuePair<string, Question>(RedisKeyHelper.GetQuestionKey(x.Id), x));

            await redisDatabase.StringSetAsync(questionKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(userQuestionStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(allUserQuestions);
        }
    }
}