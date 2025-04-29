using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces.Service;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores tags by questions ids
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupTagDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<long, Tag>(batchScheduler, options)
{
    protected override async Task<ILookup<long, Tag>> LoadGroupedBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var tagService = scope.ServiceProvider.GetRequiredService<IGetTagService>();

        var result = await tagService.GetQuestionsTagsAsync(keys);

        if (!result.IsSuccess)
            return Enumerable.Empty<KeyValuePair<long, IEnumerable<Tag>>>()
                .SelectMany(x => x.Value.Select(y => new { x.Key, Tag = y }))
                .ToLookup(x => x.Key, x => x.Tag);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Tag = y }))
            .ToLookup(x => x.Key, x => x.Tag);

        return lookup;
    }
}