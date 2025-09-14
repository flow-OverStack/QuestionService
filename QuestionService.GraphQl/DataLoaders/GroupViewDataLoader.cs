using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores views by questions ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupViewDataLoader(
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

        var result = await viewService.GetQuestionsViewsAsync(keys, cancellationToken);

        if (!result.IsSuccess)
            return Enumerable.Empty<KeyValuePair<long, IEnumerable<View>>>()
                .SelectMany(x => x.Value.Select(y => new { x.Key, View = y }))
                .ToLookup(x => x.Key, x => x.View);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, View = y }))
            .ToLookup(x => x.Key, x => x.View);

        return lookup;
    }
}