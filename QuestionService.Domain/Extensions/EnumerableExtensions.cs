namespace QuestionService.Domain.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    ///     Filters a sequence of key-value pairs where each value is a collection,
    ///     removing elements from each collection if their transformed representation
    ///     appears more than <paramref name="maxOccurrences"/> times across all collections.
    /// </summary>
    /// <param name="source">The input key-value collection where each value is a sequence.</param>
    /// <param name="valueSelector">A function that projects each element into a comparison key.</param>
    /// <param name="maxOccurrences">The maximum allowed number of occurrences of a value across all sequences.</param>
    /// <returns>A new sequence of key-value pairs with filtered value sequences.</returns>
    public static IEnumerable<KeyValuePair<TKey, IEnumerable<TElement>>> FilterByMaxValueOccurrences<TKey, TElement>(
        this IEnumerable<KeyValuePair<TKey, IEnumerable<TElement>>> source, Func<TElement, TElement> valueSelector,
        int maxOccurrences) where TElement : notnull where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);

        var keyValuePairs = source.ToList();

        var valueCounts = keyValuePairs
            .SelectMany(pair => pair.Value)
            .Select(valueSelector)
            .GroupBy(key => key)
            .ToDictionary(g => g.Key, g => g.Count());

        return keyValuePairs.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.Where(value => valueCounts[valueSelector(value)] <= maxOccurrences)
        );
    }
}