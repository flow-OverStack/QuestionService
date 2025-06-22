using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GraphQl.DataLoaders;

public class VoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<VoteDto, Vote>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<VoteDto, Vote>> LoadBatchAsync(IReadOnlyList<VoteDto> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var voteService = scope.ServiceProvider.GetRequiredService<IGetVoteService>();

        var result = await voteService.GetByDtosAsync(keys, cancellationToken);

        var dictionary = new Dictionary<VoteDto, Vote>();

        if (!result.IsSuccess)
            return dictionary.AsReadOnly();

        dictionary = result.Data.ToDictionary(x => new VoteDto(x.QuestionId, x.UserId), x => x);

        return dictionary.AsReadOnly();
    }
}