using StrawberryShake;

namespace QuestionService.Domain.Exceptions.GraphQl;

public class GraphQlFetchException : Exception
{
    public GraphQlFetchException(string message) : base($"GraphQl fetch exception: {message}")
    {
    }

    public GraphQlFetchException(IEnumerable<IClientError> collection) : base(
        $"GraphQl fetch exception: {EnumerableErrorsToString(collection)}")
    {
    }

    private static string EnumerableErrorsToString(IEnumerable<IClientError> collection)
    {
        return collection.Aggregate(string.Empty, (current, error) => current + error.Message + "\n");
    }
}