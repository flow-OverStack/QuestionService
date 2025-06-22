using Newtonsoft.Json;
using QuestionService.Domain.Interfaces.Provider;
using StackExchange.Redis;

namespace QuestionService.Cache.Providers;

public class RedisCacheProvider(IDatabase redisDatabase) : ICacheProvider
{
    private const string RedisErrorMessage = "An exception occurred while executing the Redis command.";

    private const string AddToSetsScript = """
                                           for i = 1, #KEYS do
                                               redis.call('SADD', KEYS[i], ARGV[i])
                                           end
                                           return true
                                           """;

    public async Task SetsAddAtomicallyAsync(IEnumerable<KeyValuePair<string, string>> keyValueMap,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keyValueMap);

        var keys = new List<RedisKey>();
        var values = new List<RedisValue>();

        foreach (var kvp in keyValueMap)
        {
            ArgumentNullException.ThrowIfNull(kvp);
            ArgumentException.ThrowIfNullOrWhiteSpace(kvp.Key);
            ArgumentException.ThrowIfNullOrWhiteSpace(kvp.Value);

            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }

        if (keys.Count == 0 || values.Count == 0) return;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.ScriptEvaluateAsync(AddToSetsScript, keys.ToArray(), values.ToArray());

        if (result.IsNull || !(bool)result)
            throw new RedisException(RedisErrorMessage);
    }

    public async Task<long> SetsAddAsync(IEnumerable<KeyValuePair<string, IEnumerable<string>>> keysWithValues,
        int timeToLiveInSeconds, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var keyValuePairs = keysWithValues.Where(x => x.Value.Any()).ToList();
        var setAddTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.SetAddAsync(x.Key, x.Value.Select(y => new RedisValue(y.ToString())).ToArray());
        });
        var keyExpiresTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.KeyExpireAsync(x.Key,
                TimeSpan.FromSeconds(timeToLiveInSeconds));
        });

        var setAddResult = await Task.WhenAll(setAddTasks);
        var keyExpiresResult = await Task.WhenAll(keyExpiresTasks);

        if (keyExpiresResult.Any(x => !x))
            throw new RedisException(RedisErrorMessage);

        return setAddResult.Sum();
    }

    public async Task<IEnumerable<string>> SetStringMembersAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }

    public async Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> SetsStringMembersAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var tasks = keys.Distinct().Select(async key =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var values = await SetStringMembersAsync(key, cancellationToken);
            return new KeyValuePair<string, IEnumerable<string>>(key, values);
        });

        var results = await Task.WhenAll(tasks);

        return results;
    }

    public async Task<long> SetsRemoveAsync(IEnumerable<KeyValuePair<string, IEnumerable<string>>> keyValueMap,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keyValueMap);

        var tasks = keyValueMap.Select(x =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(x.Key);
            ArgumentNullException.ThrowIfNull(x.Value);

            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.SetRemoveAsync((RedisKey)x.Key, x.Value.Select(v => new RedisValue(v)).ToArray());
        });

        var results = await Task.WhenAll(tasks);

        return results.Sum();
    }

    public async Task StringSetAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> keysWithValues,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var redisKeyWithValues = keysWithValues.DistinctBy(x => x.Key).Select(x =>
        {
            var jsonValue = JsonConvert.SerializeObject(x.Value);
            return new KeyValuePair<RedisKey, RedisValue>(x.Key, new RedisValue(jsonValue));
        });

        var tasks = redisKeyWithValues.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.StringSetAsync(x.Key, x.Value, TimeSpan.FromSeconds(timeToLiveInSeconds));
        });

        var result = await Task.WhenAll(tasks);

        if (result.Any(x => !x))
            throw new RedisException(RedisErrorMessage);
    }

    public async Task<IEnumerable<T>> GetJsonParsedAsync<T>(IEnumerable<string> keys,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(keys);

        var redisKeys = keys.Distinct().Select(x => (RedisKey)x).ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKeys);

        var jsonResult = new List<T>();

        foreach (var value in result)
        {
            if (value.IsNull) continue;

            var jsonValue = JsonConvert.DeserializeObject<T>(value.ToString());

            if (jsonValue == null) continue;

            jsonResult.Add(jsonValue);
        }

        return jsonResult;
    }

    public Task KeyDeleteAsync(IEnumerable<string> key, CancellationToken cancellationToken = default) =>
        redisDatabase.KeyDeleteAsync(key.Select(x => (RedisKey)x).ToArray());

    public Task<long> SetRemoveAsync(string key, IEnumerable<string> values,
        CancellationToken cancellationToken = default) =>
        redisDatabase.SetRemoveAsync((RedisKey)key, values.Select(x => (RedisValue)x).ToArray());
}