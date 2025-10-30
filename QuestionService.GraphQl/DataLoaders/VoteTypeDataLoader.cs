using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

public class VoteTypeDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<long, VoteType>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, VoteType>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var voteTypeService = scope.ServiceProvider.GetRequiredService<IGetVoteTypeService>();

        var result = await voteTypeService.GetByIdsAsync(keys, cancellationToken);

        var dictionary = new Dictionary<long, VoteType>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => x.Id, x => x);

        return dictionary.AsReadOnly();
    }
}