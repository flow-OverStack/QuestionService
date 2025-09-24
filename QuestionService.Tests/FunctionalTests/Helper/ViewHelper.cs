using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class ViewHelper
{
    public static Task InsertViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetViews();

        var keyValueMap = views.Keys.Select(x =>
                new KeyValuePair<string, IEnumerable<string>>(x.ToString(), views.Values.Select(y => y.ToString())))
            .ToList();
        keyValueMap.Add(
            new KeyValuePair<string, IEnumerable<string>>("view:questions", views.Keys.Select(x => x.ToString())));

        return redisDatabase.SetsAddAsync(keyValueMap);
    }

    public static Task InsertInvalidValuesViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetInvalidValuesViews();

        var keyValueMap = views.Keys.Select(x =>
                new KeyValuePair<string, IEnumerable<string>>(x.ToString(), views.Values.Select(y => y.ToString())))
            .ToList();
        keyValueMap.Add(
            new KeyValuePair<string, IEnumerable<string>>("view:questions", views.Keys.Select(x => x.ToString())));

        return redisDatabase.SetsAddAsync(keyValueMap);
    }
}