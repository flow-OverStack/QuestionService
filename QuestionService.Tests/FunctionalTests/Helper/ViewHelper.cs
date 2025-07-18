using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Helper;

internal static class ViewHelper
{
    public static async Task InsertViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetViews();

        var keyValueMap = new List<KeyValuePair<string, string>>();
        foreach (var key in views.Keys)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            keyValueMap.Add(new KeyValuePair<string, string>("view:questions", key!));
            foreach (var value in views.Values)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                keyValueMap.Add(new KeyValuePair<string, string>(key!, value!));
            }
        }

        await redisDatabase.SetsAddAtomicallyAsync(keyValueMap);
    }

    public static async Task InsertInvalidValuesViews(this ICacheProvider redisDatabase)
    {
        var views = ViewConfiguration.GetInvalidValuesViews();

        var keyValueMap = new List<KeyValuePair<string, string>>();
        foreach (var key in views.Keys)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            keyValueMap.Add(new KeyValuePair<string, string>("view:questions", key!));
            foreach (var value in views.Values)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(value);
                keyValueMap.Add(new KeyValuePair<string, string>(key!, value!));
            }
        }

        await redisDatabase.SetsAddAtomicallyAsync(keyValueMap);
    }
}