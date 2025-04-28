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
    /// <exception cref="RedisException"></exception>
    public static async Task AddToSetsAsync(this IDatabase redisDatabase,
        IEnumerable<KeyValuePair<string, string>> keyValueMap, CancellationToken cancellationToken = default)
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

        if (!keys.Any() || !values.Any()) return;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.ScriptEvaluateAsync(AddToSetsScript, keys.ToArray(), values.ToArray());

        if (result.IsNull || !(bool)result)
            throw new RedisException("An exception occured while executing the redis command.");
    }
}