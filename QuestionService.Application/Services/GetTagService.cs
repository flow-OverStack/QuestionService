using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetTagService(IBaseRepository<Tag> tagRepository, IBaseRepository<Question> questionRepository)
    : IGetTagService
{
    public async Task<CollectionResult<Tag>> GetAllAsync()
    {
        var tags = await tagRepository.GetAll().ToListAsync();

        return CollectionResult<Tag>.Success(tags, tags.Count);
    }

    public async Task<CollectionResult<Tag>> GetByNamesAsync(IEnumerable<string> names)
    {
        var tags = await tagRepository.GetAll()
            .Where(x => names.Contains(x.Name))
            .ToListAsync();
        var totalCount = await tagRepository.GetAll().CountAsync();

        if (!tags.Any())
            return names.Count() switch
            {
                <= 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound),
                > 1 => CollectionResult<Tag>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound)
            };

        return CollectionResult<Tag>.Success(tags, tags.Count, totalCount);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>> GetQuestionsTagsAsync(
        IEnumerable<long> questionIds)
    {
        var groupedTags = await questionRepository.GetAll()
            .Where(x => questionIds.Contains(x.Id))
            .Include(x => x.Tags)
            .Select(x => new KeyValuePair<long, IEnumerable<Tag>>(x.Id, x.Tags))
            .ToListAsync();

        if (!groupedTags.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Failure(ErrorMessage.TagsNotFound,
                (int)ErrorCodes.TagsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Tag>>>.Success(groupedTags, groupedTags.Count);
    }
}