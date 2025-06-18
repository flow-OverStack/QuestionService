using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetQuestionService(
    IBaseRepository<Question> questionRepository,
    IBaseRepository<Tag> tagRepository)
    : IGetQuestionService
{
    public Task<QueryableResult<Question>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var questions = questionRepository.GetAll();

        // Since there's can be no questions it is not exception to have no questions
        return Task.FromResult(QueryableResult<Question>.Success(questions));
    }

    public async Task<CollectionResult<Question>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var questions = await questionRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        if (questions.Count == 0)
            return ids.Count() switch
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
        var groupedQuestions = await tagRepository.GetAll()
            .Where(x => tagIds.Contains(x.Id))
            .Include(x => x.Questions)
            .Select(x => new KeyValuePair<long, IEnumerable<Question>>(x.Id, x.Questions))
            .ToListAsync(cancellationToken);

        if (groupedQuestions.Count == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Question>>>> GetUsersQuestionsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var questions = await questionRepository.GetAll()
            .Where(x => userIds.Contains(x.UserId))
            .ToListAsync(cancellationToken);

        var groupedQuestions = questions
            .GroupBy(x => x.UserId)
            .Select(x => new KeyValuePair<long, IEnumerable<Question>>(x.Key, x.ToList()))
            .ToList();

        if (groupedQuestions.Count == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Failure(ErrorMessage.QuestionsNotFound,
                (int)ErrorCodes.QuestionsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Question>>>.Success(groupedQuestions);
    }
}