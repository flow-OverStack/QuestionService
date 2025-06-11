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
    ///     Atomically adds a collection of key-value pairs to sets in Redis.
    ///     Each key is associated with a set, and the corresponding value is added to the set.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="keyValueMap">A collection of key-value pairs where keys are Redis set names and values are items to be added to the sets.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <exception cref="RedisException">Thrown when the Redis command execution fails.</exception>
    public static async Task SetsAddAtomicallyAsync(this IDatabase redisDatabase,
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

        if (keys.Count == 0 || values.Count == 0) return;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.ScriptEvaluateAsync(AddToSetsScript, keys.ToArray(), values.ToArray());

        if (result.IsNull || !(bool)result)
            throw new RedisException("An exception occurred while executing the Redis command.");
    }

    /// <summary>
    ///     Retrieves the members of a set in Redis as a collection of strings.
    ///     The set is identified by the provided Redis key.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="key">The Redis key that represents the set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A collection of strings representing the members of the set.</returns>
    public static async Task<IEnumerable<string>> SetStringMembersAsync(this IDatabase redisDatabase, RedisKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }

    /// <summary>
    ///     Retrieves the members of multiple Redis sets, returning them as key-value pairs where
    ///     each key is a set identifier, and the value is the collection of members for that set.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="keys">A collection of Redis set keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A collection of key-value pairs, where the key is the set key and the value is the collection of its members.</returns>
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

    /// <summary>
    ///     Not atomically removes specified members from the sets in Redis. The sets and members are identified
    ///     by a collection of key-value pairs where each key is a Redis set and the value is the
    ///     list of members to remove.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="keyValueMap">A collection of key-value pairs where the key is a Redis set key and the value is a collection of members to remove from the set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>The total number of members removed from the sets.</returns>
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