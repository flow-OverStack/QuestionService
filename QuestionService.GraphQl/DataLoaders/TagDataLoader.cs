using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Interfaces.Service;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.DataLoaders;

public class TagDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<string, Tag>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<string, Tag>> LoadBatchAsync(IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var tagService = scope.ServiceProvider.GetRequiredService<IGetTagService>();

        var result = await tagService.GetByNamesAsync(keys);

        var dictionary = new Dictionary<string, Tag>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Name, x => x);

        return dictionary.AsReadOnly();
    }
}