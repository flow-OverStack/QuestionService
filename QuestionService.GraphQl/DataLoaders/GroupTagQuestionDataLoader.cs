using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

/// <summary>
///     Data loader that stores questions by tags names 
/// </summary>
/// <param name="batchScheduler"></param>
/// <param name="options"></param>
/// <param name="scopeFactory"></param>
public class GroupTagQuestionDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : GroupedDataLoader<string, Question>(batchScheduler, options)
{
    protected override async Task<ILookup<string, Question>> LoadGroupedBatchAsync(IReadOnlyList<string> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var questionService = scope.ServiceProvider.GetRequiredService<IGetQuestionService>();

        var result = await questionService.GetQuestionsWithTagsAsync(keys);

        if (!result.IsSuccess)
            return Enumerable.Empty<KeyValuePair<string, IEnumerable<Question>>>()
                .SelectMany(x => x.Value.Select(y => new { x.Key, Question = y }))
                .ToLookup(x => x.Key, x => x.Question);

        var lookup = result.Data
            .SelectMany(x => x.Value.Select(y => new { x.Key, Question = y }))
            .ToLookup(x => x.Key, x => x.Question);

        return lookup;
    }
}