using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores views by users ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupUserViewDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, View>(batchScheduler, options)
{
    protected override async Task<ILookup<long, View>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var viewService = scope.ServiceProvider.GetRequiredService<IGetViewService>();

        var result = await viewService.GetUsersViewsAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<IGrouping<long, View>>().ToLookup(_ => 0L, _ => default(View)!); // Empty lookup

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, View = y }))
            .ToLookup(x => x.Key, x => x.View);

        return lookup;
    }
}