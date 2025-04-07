using Microsoft.EntityFrameworkCore;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Result;

namespace QuestionService.Application.Services;

public class GetViewService(IBaseRepository<View> viewRepository) : IGetViewService
{
    public async Task<CollectionResult<View>> GetAllAsync()
    {
        var views = await viewRepository.GetAll().ToListAsync();

        return CollectionResult<View>.Success(views, views.Count);
    }

    public async Task<BaseResult<View>> GetByIdAsync(long id)
    {
        var view = await viewRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (view == null)
            return BaseResult<View>.Failure(ErrorMessage.ViewNotFound, (int)ErrorCodes.ViewNotFound);

        return BaseResult<View>.Success(view);
    }

    public async Task<CollectionResult<View>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var views = await viewRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();
        var totalCount = await viewRepository.GetAll().CountAsync();

        if (!views.Any())
            return CollectionResult<View>.Failure(ErrorMessage.ViewsNotFound, (int)ErrorCodes.ViewsNotFound);

        return CollectionResult<View>.Success(views, views.Count, totalCount);
    }
}