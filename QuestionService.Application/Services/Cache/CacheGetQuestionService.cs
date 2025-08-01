using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetQuestionService(IQuestionCacheRepository cacheRepository, GetQuestionService inner)
    : IGetQuestionService
{
    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var questions = (await cacheRepository.GetByIdsAsync(idsArray, cancellationToken)).ToArray();

        if (questions.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionNotFound,
                    (int)ErrorCodes.QuestionNotFound),
                > 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionsNotFound,
                    (int)ErrorCodes.QuestionsNotFound)
            };

        return CollectionResult<Question>.Success(questions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<long> tagIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = (await cacheRepository.GetQuestionsWithTagsAsync(tagIds, cancellationToken)).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = (await cacheRepository.GetUsersQuestionsAsync(userIds, cancellationToken)).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }
}