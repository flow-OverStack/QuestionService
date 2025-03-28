using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetQuestionService(IBaseRepository<Question> questionRepository, IBaseRepository<Tag> tagRepository)
    : IGetQuestionService
{
    public async Task<CollectionResult<Question>> GetAllAsync()
    {
        var questions = await questionRepository.GetAll().ToListAsync();

        if (!questions.Any())
            return CollectionResult<Question>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<Question>.Success(questions, questions.Count);
    }

    public async Task<BaseResult<Question>> GetByIdAsync(long id)
    {
        var question = await questionRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (question == null)
            return BaseResult<Question>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        return BaseResult<Question>.Success(question);
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
}