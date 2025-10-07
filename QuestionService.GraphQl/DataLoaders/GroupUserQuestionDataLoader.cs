using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores questions by users ids
/// </summary>
public class GroupUserQuestionDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Question>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Question>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var questionService = scope.ServiceProvider.GetRequiredService<IGetQuestionService>();

        var result = await questionService.GetUsersQuestionsAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, Question>>()
                .ToLookup(_ => 0L, _ => default(Question)!); // Empty lookup

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Question = y }))
            .ToLookup(x => x.Key, x => x.Question);

        return lookup;
    }
}