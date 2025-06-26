using Microsoft.Extensions.Options;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services.Cache;

public class CacheGetQuestionService(
    IBaseCacheRepository<Question, long> cacheRepository,
    GetQuestionService inner,
    IOptions<RedisSettings> redisSettings) : IGetQuestionService
{
    private readonly RedisSettings _redisSettings = redisSettings.Value;

    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var questions = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

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
        var groupedQuestions = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            tagIds,
            CacheKeyHelper.GetTagQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetQuestionsWithTagsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var groupedQuestions = (await cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserQuestionsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await inner.GetUsersQuestionsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedQuestions.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }
}