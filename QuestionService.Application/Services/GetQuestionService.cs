using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetQuestionService(
    IBaseRepository<Question> questionRepository,
    IBaseRepository<Tag> tagRepository,
    IEntityProvider<UserDto> userProvider)
    : IGetQuestionService
{
    public async Task<CollectionResult<Question>> GetAllAsync()
    {
        var questions = await questionRepository.GetAll().ToListAsync();

        // Since there's can be no questions it is not exception to have no questions
        return CollectionResult<Question>.Success(questions, questions.Count);
    }

    public async Task<BaseResult<Question>> GetByIdAsync(long id)
    {
        var question = await questionRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (question == null)
            return BaseResult<Question>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        return BaseResult<Question>.Success(question);
    }

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var questions = await questionRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();
        var totalCount = await questionRepository.GetAll().CountAsync();

        if (!questions.Any())
            return CollectionResult<Question>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<Question>.Success(questions, questions.Count, totalCount);
    }

    public async Task<CollectionResult<Question>> GetQuestionsWithTag(string tagName)
    {
        var tag = await tagRepository.GetAll().Include(x => x.Questions).FirstOrDefaultAsync(x => x.Name == tagName);

        if (tag == null)
            return CollectionResult<Question>.Failure(ErrorMessage.TagNotFound,
                (int)ErrorCodes.TagNotFound);

        var questions = tag.Questions;

        return CollectionResult<Question>.Success(questions, questions.Count);
    }

    public async Task<CollectionResult<Question>> GetUserQuestions(long userId)
    {
        var userExists = await userProvider.GetByIdAsync(userId) != null;
        if (!userExists)
            return CollectionResult<Question>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var questions = await questionRepository.GetAll().Where(x => x.UserId == userId).ToListAsync();
        var totalCount = await questionRepository.GetAll().CountAsync();

        return CollectionResult<Question>.Success(questions, questions.Count, totalCount);
    }

    public async Task<BaseResult<QuestionViewsDto>> GetQuestionViewsCount(long questionId)
    {
        var question = await questionRepository.GetAll().Include(x => x.Views).Select(x => new
            {
                x.Id,
                ViewsCount = x.Views.Count,
            })
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (question == null)
            return BaseResult<QuestionViewsDto>.Failure(ErrorMessage.QuestionNotFound,
                (int)ErrorCodes.QuestionNotFound);

        var dto = new QuestionViewsDto
        {
            QuestionId = question.Id,
            Views = question.ViewsCount
        };
        return BaseResult<QuestionViewsDto>.Success(dto);
    }
}