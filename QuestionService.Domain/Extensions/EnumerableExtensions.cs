namespace QuestionService.Domain.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Filters a sequence of values, removing elements if their transformed representation
    ///     appears more than the <paramref name="maxOccurrences"/> number of times.
    /// </summary>
    /// <typeparam name="TValue">The type of the values in the sequence.</typeparam>
    /// <typeparam name="TCompare">The type of the comparison key produced by the valueSelector.</typeparam>
    /// <param name="source">The input sequence of values.</param>
    /// <param name="valueSelector">A function that projects each element into a comparison key.</param>
    /// <param name="maxOccurrences">The maximum allowed number of occurrences of a value.</param>
    /// <returns>A new sequence with filtered values.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<TValue> FilterByMaxValueOccurrences<TValue, TCompare>(this IEnumerable<TValue> source,
        Func<TValue, TCompare> valueSelector, int maxOccurrences) where TValue : notnull where TCompare : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        var values = source.ToArray();

        var valueCounts = values
            .Select(valueSelector)
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());

        return values.Where(value => valueCounts[valueSelector(value)] <= maxOccurrences);
    }

    /// <summary>
    ///     Projects each key in a sequence of grouped key-value pairs into an intermediate result,
    ///     and then projects each value in the associated value collection into a final result using the key result.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the source sequence.</typeparam>
    /// <typeparam name="TValue">The type of the values in the inner collections.</typeparam>
    /// <typeparam name="TKeyResult">The result type produced from each key.</typeparam>
    /// <typeparam name="TResult">The result type produced from each key-value pair.</typeparam>
    /// <param name="source">The source collection of key-value pairs with grouped values.</param>
    /// <param name="keySelector">A function to transform each key to an intermediate result.</param>
    /// <param name="valueSelector">A function to project each value using the result of the corresponding key.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TResult}"/> that contains the transformed values for each grouped entry.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="source"/>, <paramref name="keySelector"/>, or <paramref name="valueSelector"/> is <c>null</c>.
    /// </exception>
    public static IEnumerable<TResult> SelectManyFromGroupedValues<TKey, TValue, TKeyResult, TResult>(
        this IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>> source,
        Func<TKey, TKeyResult> keySelector,
        Func<TKeyResult, TValue, TResult> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(valueSelector);

        return source.SelectManyFromGroupedValuesImpl(keySelector, valueSelector);
    }

    private static IEnumerable<TResult> SelectManyFromGroupedValuesImpl<TKey, TValue, TKeyResult, TResult>(
        this IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>> source,
        Func<TKey, TKeyResult> keySelector,
        Func<TKeyResult, TValue, TResult> valueSelector)
    {
        foreach (var kvp in source)
        {
            var keyResult = keySelector(kvp.Key);

            foreach (var value in kvp.Value)
            {
                var result = valueSelector(keyResult, value);
                yield return result;
            }
        }
    }
}