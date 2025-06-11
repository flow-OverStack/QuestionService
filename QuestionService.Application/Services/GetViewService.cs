using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;

namespace QuestionService.Application.Services;

public class GetViewService(IBaseRepository<View> viewRepository, IBaseRepository<Question> questionRepository)
    : IGetViewService
{
    public Task<QueryableResult<View>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var views = viewRepository.GetAll();

        return Task.FromResult(QueryableResult<View>.Success(views));
    }


    public async Task<CollectionResult<View>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var views = await viewRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        if (views.Count == 0)
            return ids.Count() switch
            {
                <= 1 => CollectionResult<View>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound),
                > 1 => CollectionResult<View>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound),
            };

        return CollectionResult<View>.Success(views);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetUsersViewsAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var views = await viewRepository.GetAll()
            .Where(x => x.UserId != null && userIds.Contains((long)x.UserId))
            .ToListAsync(cancellationToken);

        var groupedViews = views
            .GroupBy(x => (long)x.UserId!)
            .Select(x => new KeyValuePair<long, IEnumerable<View>>(x.Key, x))
            .ToList();


        if (groupedViews.Count == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<View>>>> GetQuestionsViewsAsync(
        IEnumerable<long> questionIds, CancellationToken cancellationToken = default)
    {
        var groupedViews = await questionRepository.GetAll()
            .Where(x => questionIds.Contains(x.Id))
            .Include(x => x.Views)
            .Select(x => new KeyValuePair<long, IEnumerable<View>>(x.Id, x.Views))
            .ToListAsync(cancellationToken);

        if (groupedViews.Count == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Failure(ErrorMessage.ViewsNotFound,
                (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<View>>>.Success(groupedViews);
    }
}