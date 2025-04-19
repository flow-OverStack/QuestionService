using Microsoft.Extensions.DependencyInjection;
using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;

namespace QuestionService.GraphQl.DataLoaders;

public class VoteDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory scopeFactory)
    : BatchDataLoader<GetVoteDto, Vote>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<GetVoteDto, Vote>> LoadBatchAsync(IReadOnlyList<GetVoteDto> keys,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var voteService = scope.ServiceProvider.GetRequiredService<IGetVoteService>();

        var result = await voteService.GetByDtosAsync(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var dictionary = new Dictionary<GetVoteDto, Vote>();
        result.Data.ToList().ForEach(x => dictionary.Add(new GetVoteDto(x.QuestionId, x.UserId), x));

        return dictionary.AsReadOnly();
    }
}