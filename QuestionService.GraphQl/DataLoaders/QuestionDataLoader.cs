using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;

namespace QuestionService.GraphQl.DataLoaders;

// The data loader is not used yet because 
// question is not resolved by any other entity yet
public class QuestionDataLoader(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IGetQuestionService questionService)
    : BatchDataLoader<long, Question>(batchScheduler, options)
{
    protected override async Task<IReadOnlyDictionary<long, Question>> LoadBatchAsync(IReadOnlyList<long> keys,
        CancellationToken cancellationToken)
    {
        var result = await questionService.GetByIdsAsync(keys);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        var dictionary = new Dictionary<long, Question>();
        result.Data.ToList().ForEach(x => dictionary.Add(x.Id, x));

        return dictionary.AsReadOnly();
    }
}