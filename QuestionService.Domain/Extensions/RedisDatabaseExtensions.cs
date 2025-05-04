using StackExchange.Redis;

namespace QuestionService.Domain.Extensions;

public static class RedisDatabaseExtensions
{
    private const string AddToSetsScript = """
                                           for i = 1, #KEYS do
                                               redis.call('SADD', KEYS[i], ARGV[i])
                                           end
                                           return true
                                           """;

    /// <summary>
    ///     Adds collection of key-value pairs to sets
    /// </summary>
    /// <param name="redisDatabase"></param>
    /// <param name="keyValueMap"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="RedisException"></exception>
    public static async Task SetsAddAsync(this IDatabase redisDatabase,
        IEnumerable<KeyValuePair<string, string>> keyValueMap, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
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

        if (!keys.Any() || !values.Any()) return;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.ScriptEvaluateAsync(AddToSetsScript, keys.ToArray(), values.ToArray());

        if (result.IsNull || !(bool)result)
            throw new RedisException("An exception occured while executing the redis command.");
    }


    public static async Task<IEnumerable<string>> SetStringMembersAsync(this IDatabase redisDatabase, RedisKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }


    public static async Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> SetsMembersAsync(
        this IDatabase redisDatabase, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keys);


        var tasks = keys.Distinct().Select(async key =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var values = await redisDatabase.SetStringMembersAsync(key, cancellationToken);
            return new KeyValuePair<string, IEnumerable<string>>(key, values);
        });

        var results = await Task.WhenAll(tasks);

        return results;
    }

    public static async Task<long> SetsRemoveAsync(this IDatabase redisDatabase,
        IEnumerable<KeyValuePair<RedisKey, IEnumerable<RedisValue>>> keyValueMap,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keyValueMap);

        var tasks = keyValueMap.Select(x =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(x.Key);
            ArgumentNullException.ThrowIfNull(x.Value);

            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.SetRemoveAsync(x.Key, x.Value.ToArray());
        });

        var results = await Task.WhenAll(tasks);

        return results.Sum();
    }
}