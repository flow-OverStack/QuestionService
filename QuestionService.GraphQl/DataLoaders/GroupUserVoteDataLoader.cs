using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores votes by user ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupUserVoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Vote>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Vote>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var questionService = scope.ServiceProvider.GetRequiredService<IGetVoteService>();

        var result = await questionService.GetUsersVotesAsync(keys);

        if (!result.IsSuccess)
            return Enumerable.Empty<KeyValuePair<long, IEnumerable<Vote>>>()
                .SelectMany(x => x.Value.Select(y => new { x.Key, Vote = y }))
                .ToLookup(x => x.Key, x => x.Vote);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Vote = y }))
            .ToLookup(x => x.Key, x => x.Vote);

        return lookup;
    }
}