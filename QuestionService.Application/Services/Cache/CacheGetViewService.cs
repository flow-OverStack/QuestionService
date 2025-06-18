using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;
using StackExchange.Redis;

namespace QuestionService.Application.Services.Cache;

public class CacheGetViewService(
    GetViewService inner,
    IDatabase redisDatabase,
    IOptions<RedisSettings> redisSettings) : IGetViewService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<View>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsList = ids.ToList();

        try
        {
            var keys = idsList.Select(RedisKeyHelper.GetViewKey);
            var views = (await redisDatabase.GetJsonParsedAsync<View>(keys, cancellationToken)).ToList();

            var missingIds = idsList.Except(views.Select(x => x.Id)).ToList();

            if (missingIds.Count > 0) return await GetFromInnerAndCacheAsync(missingIds, views);

            return CollectionResult<View>.Success(views);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<View>> GetFromInnerAndCacheAsync(IEnumerable<long> missingIds,
            IEnumerable<View> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetByIdsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<View>.Success(alreadyCachedList)
                    : result;

            var allViews = result.Data.UnionBy(alreadyCachedList, x => x.Id).ToList();

            var viewKeys = allViews.Select(x =>
                new KeyValuePair<string, View>(RedisKeyHelper.GetViewKey(x.Id), x));

            await redisDatabase.StringSetAsync(viewKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<View>.Success(allViews);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var idsList = userIds.ToList();

        try
        {
            var userViewKeys = idsList.Select(RedisKeyHelper.GetUserViewsKey);
            var userViewStringIds =
                (await redisDatabase.SetsStringMembersAsync(userViewKeys, cancellationToken)).Where(x => x.Value.Any());

            var userViewsIds = userViewStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var viewKeys = userViewsIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetViewKey)).Distinct();
            var views = await redisDatabase.GetJsonParsedAsync<View>(viewKeys, cancellationToken);

            var userViews = userViewsIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<View>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => views.FirstOrDefault(view => view.Id == v))
                            .Where(v => v != null)!))
                .ToList();

            var missingUserViews = idsList.Except(userViewsIds.Select(x => x.Key)).Distinct().ToList();
            var cachedUserViews = new List<KeyValuePair<long, IEnumerable<View>>>();
            foreach (var userViewId in userViewsIds)
            {
                // Keys in userViewsIds are guaranteed to be in userViews

                var actualUserView = userViews.First(x => x.Key == userViewId.Key);

                if (userViewId.Value.Except(actualUserView.Value.Select(x => x.Id)).Any())
                    missingUserViews.Add(userViewId.Key);
                else
                    cachedUserViews.Add(actualUserView);
            }

            if (missingUserViews.Count > 0)
                return await GetFromInnerAndCacheAsync(missingUserViews, cachedUserViews);

            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(userViews);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<View>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetUsersViewsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(alreadyCachedList)
                    : result;

            var allUserViews = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var userViewStringIds = allUserViews.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetUserViewsKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var views = allUserViews.SelectMany(x => x.Value);
            var viewKeys = views.Select(x =>
                new KeyValuePair<string, View>(RedisKeyHelper.GetViewKey(x.Id), x));

            await redisDatabase.StringSetAsync(viewKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(userViewStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(allUserViews);
        }
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var idsList = questionIds.ToList();

        try
        {
            var questionViewKeys = idsList.Select(RedisKeyHelper.GetQuestionViewsKey);
            var questionViewStringIds =
                (await redisDatabase.SetsStringMembersAsync(questionViewKeys, cancellationToken)).Where(x =>
                    x.Value.Any());

            var questionViewsIds = questionViewStringIds.Select(x =>
                new KeyValuePair<long, IEnumerable<long>>(RedisKeyHelper.GetIdFromKey(x.Key),
                    x.Value.Select(long.Parse))).ToList();

            var viewKeys = questionViewsIds.SelectMany(x => x.Value.Select(RedisKeyHelper.GetViewKey)).Distinct();
            var views = await redisDatabase.GetJsonParsedAsync<View>(viewKeys, cancellationToken);

            var questionViews = questionViewsIds.Select(kvp =>
                    new KeyValuePair<long, IEnumerable<View>>(
                        kvp.Key,
                        kvp.Value
                            .Select(v => views.FirstOrDefault(view => view.Id == v))
                            .Where(v => v != null)!))
                .ToList();

            var missingQuestionViews = idsList.Except(questionViewsIds.Select(x => x.Key)).Distinct().ToList();
            var cachedUserViews = new List<KeyValuePair<long, IEnumerable<View>>>();
            foreach (var questionViewId in questionViewsIds)
            {
                // Keys in questionViewsIds are guaranteed to be in questionViews

                var actualQuestionView = questionViews.First(x => x.Key == questionViewId.Key);

                if (questionViewId.Value.Except(actualQuestionView.Value.Select(x => x.Id)).Any())
                    missingQuestionViews.Add(questionViewId.Key);
                else
                    cachedUserViews.Add(actualQuestionView);
            }

            if (missingQuestionViews.Count > 0)
                return await GetFromInnerAndCacheAsync(missingQuestionViews, cachedUserViews);

            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(questionViews);
        }
        catch (Exception)
        {
            return await GetFromInnerAndCacheAsync(idsList, []);
        }

        async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetFromInnerAndCacheAsync(
            IEnumerable<long> missingIds, IEnumerable<KeyValuePair<long, IEnumerable<View>>> alreadyCached)
        {
            var alreadyCachedList = alreadyCached.ToList();

            var result = await inner.GetQuestionsViewsAsync(missingIds, cancellationToken);

            if (!result.IsSuccess)
                return alreadyCachedList.Count > 0
                    ? CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(alreadyCachedList)
                    : result;

            var allQuestionViews = result.Data.UnionBy(alreadyCachedList, x => x.Key).ToList();

            var questionViewStringIds = allQuestionViews.Select(kvp =>
                new KeyValuePair<string, IEnumerable<string>>(RedisKeyHelper.GetQuestionViewsKey(kvp.Key),
                    kvp.Value.Select(x => x.Id.ToString())));

            var views = allQuestionViews.SelectMany(x => x.Value);
            var viewKeys = views.Select(x =>
                new KeyValuePair<string, View>(RedisKeyHelper.GetViewKey(x.Id), x));

            await redisDatabase.StringSetAsync(viewKeys, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);
            await redisDatabase.SetsAddAsync(questionViewStringIds, _redisSettings.TimeToLiveInSeconds,
                cancellationToken);

            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(allQuestionViews);
        }
    }
}