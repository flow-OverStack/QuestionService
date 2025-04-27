using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetQuestionService(
    IBaseRepository<Question> questionRepository,
    IBaseRepository<Tag> tagRepository)
    : IGetQuestionService
{
    public async Task<CollectionResult<Question>> GetAllAsync()
    {
        var questions = await questionRepository.GetAll().ToListAsync();

        // Since there's can be no questions it is not exception to have no questions
        return CollectionResult<Question>.Success(questions, questions.Count);
    }

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var questions = await questionRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();
        var totalCount = await questionRepository.GetAll().CountAsync();

        if (!questions.Any())
            return ids.Count() switch
            {
                <= 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionNotFound,
                    (int)ErrorCodes.QuestionNotFound),
                > 1 => CollectionResult<Question>.Failure(ErrorMessage.QuestionsNotFound,
                    (int)ErrorCodes.QuestionsNotFound)
            };

        return CollectionResult<Question>.Success(questions, questions.Count, totalCount);
    }

    public async Task<CollectionResult<KeyValuePair<string, IEnumerable<Question>>>> GetQuestionsWithTagsAsync(
        IEnumerable<string> tagNames)
    {
        var groupedQuestions = await tagRepository.GetAll()
            .Where(x => tagNames.Contains(x.Name))
            .Include(x => x.Questions)
            .Select(x => new KeyValuePair<string, IEnumerable<Question>>(x.Name, x.Questions))
            .ToListAsync();

        if (!groupedQuestions.Any())
            return CollectionResult<KeyValuePair<string, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<string, IEnumerable<Question>>>.Success(groupedQuestions,
            groupedQuestions.Count);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds)
    {
        var questions = await questionRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync();

        var groupedQuestions = questions
            .GroupBy(x => x.UserId)
            .Select(x => new KeyValuePair<long, IEnumerable<Question>>(x.Key, x))
            .ToList();

        if (!groupedQuestions.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions,
            groupedQuestions.Count);
    }
}