using System.Net;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Application.Settings;
using QuestionService.Domain.Comparers;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.View;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Repository.Cache;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services;

public class ViewService(
    IViewCacheSyncRepository cacheRepository,
    IBaseRepository<Question> questionRepository,
    IBaseRepository<View> viewRepository,
    IEntityProvider<UserDto> userProvider,
    IOptions<ContentRules> contentRules,
    IOptions<EntityRules> entityRules)
    : IViewService, IViewDatabaseService
{
    private readonly ContentRules _contentRules = contentRules.Value;
    private readonly EntityRules _entityRules = entityRules.Value;

    public async Task<BaseResult<SyncedViewsDto>> SyncViewsToDatabaseAsync(
        CancellationToken cancellationToken = default)
    {
        var views = (await cacheRepository.GetValidViewsAsync(cancellationToken)).ToArray();

        var spamFilteredViews = views.FilterByMaxValueOccurrences(
            view => (view.UserId?.ToString() ?? view.UserIp)!,
            _contentRules.UserViewSpamThreshold).ToArray();

        if (spamFilteredViews.Length == 0) return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(0));

        // Filtering views to have real user and question ids and to be unique

        var predicate = PredicateBuilder.New<View>();
        predicate = spamFilteredViews.Aggregate(predicate,
            (current, local) =>
                current.Or(UniqueViewComparer.ViewEquals(local)));

        var existingViews = await viewRepository.GetAll()
            .AsExpandable()
            .Where(predicate)
            .ToHashSetAsync(new UniqueViewComparer(), cancellationToken);
        var existingQuestionIds = await questionRepository.GetAll()
            .Where(x => spamFilteredViews.Select(y => y.QuestionId).Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);
        var existingUserIds = (await userProvider.GetByIdsAsync(
                spamFilteredViews.Where(x => x.UserId != null).Select(x => (long)x.UserId!),
                cancellationToken))
            .Select(x => x.Id)
            .ToHashSet();

        // Hash sets to avoid O(n²) with Where and Any inside in-memory filtering
        var newUniqueViews = spamFilteredViews
            .Where(x => !existingViews.Contains(x)) // Checking that a view is unique, excluding all existing views
            .Where(x => existingQuestionIds.Contains(x.QuestionId)) // Checking question existence
            .Where(x => x.UserId == null || existingUserIds.Contains((long)x.UserId)) // Checking user existence
            .ToArray();

        await viewRepository.CreateRangeAsync(newUniqueViews, cancellationToken);
        await viewRepository.SaveChangesAsync(cancellationToken);

        await cacheRepository.DeleteAllViewsAsync(CancellationToken.None);

        return BaseResult<SyncedViewsDto>.Success(new SyncedViewsDto(newUniqueViews.Length));
    }

    public async Task<BaseResult> IncrementViewsAsync(IncrementViewsDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsValidData(dto))
            return BaseResult.Failure(ErrorMessage.InvalidDataFormat,
                (int)ErrorCodes.InvalidDataFormat);

        // We do not check user and question existence
        // because that may increase the request processing time
        // And this request is called frequently

        await cacheRepository.AddViewAsync(dto, cancellationToken);

        return BaseResult.Success();
    }

    private bool IsValidData(IncrementViewsDto dto)
    {
        return !StringHelper.AnyNullOrWhiteSpace(dto.UserIp, dto.UserFingerprint)
               && dto.UserFingerprint.HasMaxLength(_entityRules.UserFingerprintLength)
               && IPAddress.TryParse(dto.UserIp, out _);
    }
}