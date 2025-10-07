using QuestionService.Cache.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Tests.Configurations;
using StackExchange.Redis;

namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class ViewHelper
{
    public static Task InsertViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetViews();

        return redisDatabase.InsertViewsFromKeysAndValues(views);
    }

    public static Task InsertInvalidValuesViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetInvalidValuesViews();

        return redisDatabase.InsertViewsFromKeysAndValues(views);
    }

    private static Task InsertViewsFromKeysAndValues(this ICacheProvider redisDatabase,
        (RedisValue[] Keys, RedisValue[] Values) views)
    {
        var keyValueMap = views.Keys.Select(x =>
                new KeyValuePair<string, IEnumerable<string>>(x.ToString(), views.Values.Select(y => y.ToString())))
            .ToList();
        keyValueMap.Add(
            new KeyValuePair<string, IEnumerable<string>>(CacheKeyHelper.GetViewQuestionsKey(),
                views.Keys.Select(x => x.ToString())));

        return redisDatabase.SetsAddAsync(keyValueMap);
    }
}