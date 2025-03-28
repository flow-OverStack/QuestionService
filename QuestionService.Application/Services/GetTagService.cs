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

        if (!tags.Any())
            return CollectionResult<Tag>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        return CollectionResult<Tag>.Success(tags, tags.Count);
    }

    public async Task<BaseResult<Tag>> GetByNameAsync(string name)
    {
        var tag = await tagRepository.GetAll().FirstOrDefaultAsync(x => x.Name == name);

        if (tag == null)
            return BaseResult<Tag>.Failure(ErrorMessage.TagNotFound, (int)ErrorCodes.TagNotFound);

        return BaseResult<Tag>.Success(tag);
    }

    public async Task<CollectionResult<Tag>> GetQuestionTags(long questionId)
    {
        var question = await questionRepository.GetAll().Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (question == null)
            return CollectionResult<Tag>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        var tags = question.Tags;

        if (!tags.Any())
            return CollectionResult<Tag>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        return CollectionResult<Tag>.Success(tags, tags.Count);
    }
}