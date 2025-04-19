using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
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

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var dictionary = new Dictionary<string, Tag>();
        result.Data.ToList().ForEach(x => dictionary.Add(x.Name, x));

        return dictionary.AsReadOnly();
    }
}