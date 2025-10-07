using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores votes by questions ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupVoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Vote>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Vote>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var voteService = scope.ServiceProvider.GetRequiredService<IGetVoteService>();

        var result = await voteService.GetQuestionsVotesAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, Vote>>().ToLookup(_ => 0L, _ => default(Vote)!); // Empty lookup

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Vote = y }))
            .ToLookup(x => x.Key, x => x.Vote);

        return lookup;
    }
}