using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

public class QuestionDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, Question>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, Question>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var questionService = scope.ServiceProvider.GetRequiredService<IGetQuestionService>();

        var result = await questionService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, Question>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        result.Data.ToList().ForEach(x => dictionary.Add(x.Id, x));

        return dictionary.AsReadOnly();
    }
}