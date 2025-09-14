using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces.Service;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.DataLoaders;

public class TagDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, Tag>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, Tag>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var tagService = scope.ServiceProvider.GetRequiredService<IGetTagService>();

        var result = await tagService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, Tag>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}