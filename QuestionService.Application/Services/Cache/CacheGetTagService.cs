using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services.Cache;

public class CacheGetTagService(ITagCacheRepository cacheRepository, GetTagService inner) : IGetTagService
{
    public Task<QueryableResult<Tag>> GetAllAsync(CancellationToken cancellationToken = default) =>
        inner.GetAllAsync(cancellationToken);

    public async Task<CollectionResult<Tag>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var tags = (await cacheRepository.GetByIdsAsync(idsArray, cancellationToken)).ToArray();

        if (tags.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound),
                > 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound)
            };

        return CollectionResult<Tag>.Success(tags);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedTags = (await cacheRepository.GetQuestionsTagsAsync(questionIds, cancellationToken)).ToArray();

        if (groupedTags.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Failure(ErrorMessage.TagsNotFound,
                (int)ErrorCodes.TagsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(groupedTags);
    }
}