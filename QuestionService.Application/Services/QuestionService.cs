using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Interfaces.Producer;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Interfaces.Repository;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Results;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services;

public class QuestionService(
    IUnitOfWork unitOfWork,
    IBaseRepository<Tag> tagRepository,
    IEntityProvider<UserDto> userProvider,
    IOptions<BusinessRules> businessRules,
    IMapper mapper,
    IBaseEventProducer producer)
    : IQuestionService
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<QuestionDto>> AskQuestionAsync(long initiatorId, AskQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var user = await userProvider.GetByIdAsync(initiatorId, cancellationToken);
        if (user == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var tags = await tagRepository.GetAll().Where(x => dto.TagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != dto.TagNames.Count())
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        Question question;
        await using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                question = mapper.Map<Question>(dto);
                question.UserId = initiatorId;
                question.Tags = tags;

                await unitOfWork.Questions.CreateAsync(question, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        var questionDto = mapper.Map<QuestionDto>(question);
        return BaseResult<QuestionDto>.Success(questionDto);
    }

    public async Task<BaseResult<QuestionDto>> EditQuestionAsync(long initiatorId, EditQuestionDto dto,
        CancellationToken cancellationToken = default)
    {
        if (!IsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        var tags = await tagRepository.GetAll().Where(x => dto.TagNames.Contains(x.Name))
            .ToListAsync(cancellationToken);
        if (tags.Count != dto.TagNames.Count())
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        await using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                mapper.Map(dto, question);
                question.Tags = tags;

                unitOfWork.Questions.Update(question);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<QuestionDto>> DeleteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        unitOfWork.Questions.Remove(question);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<VoteQuestionDto>> UpvoteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < _businessRules.MinReputationToUpvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        await using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                if (vote == null)
                {
                    vote = new Vote
                    {
                        QuestionId = question.Id,
                        UserId = initiator.Id,
                        ReputationChange = _businessRules.UpvoteReputationChange
                    };

                    await unitOfWork.Votes.CreateAsync(vote, cancellationToken);
                }
                else
                {
                    if (vote.ReputationChange >= _businessRules.UpvoteReputationChange)
                        return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                            (int)ErrorCodes.VoteAlreadyGiven);

                    vote.ReputationChange = _businessRules.UpvoteReputationChange;
                    unitOfWork.Votes.Update(vote);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await producer.ProduceAsync(initiator.Id, BaseEventType.QuestionUpvote, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    public async Task<BaseResult<VoteQuestionDto>> DownvoteQuestionAsync(long initiatorId, long questionId,
        CancellationToken cancellationToken = default)
    {
        var initiator = await userProvider.GetByIdAsync(initiatorId, cancellationToken);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId, cancellationToken);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < _businessRules.MinReputationToDownvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        await using (var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                if (vote == null)
                {
                    vote = new Vote
                    {
                        QuestionId = question.Id,
                        UserId = initiator.Id,
                        ReputationChange = _businessRules.UpvoteReputationChange
                    };

                    await unitOfWork.Votes.CreateAsync(vote, cancellationToken);
                }
                else
                {
                    if (vote.ReputationChange <= _businessRules.DownvoteReputationChange)
                        return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                            (int)ErrorCodes.VoteAlreadyGiven);

                    vote.ReputationChange = _businessRules.DownvoteReputationChange;
                    unitOfWork.Votes.Update(vote);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                await producer.ProduceAsync(question.UserId, BaseEventType.QuestionDownvote, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    private static bool HasAccess(UserDto initiator, Question toQuestion)
    {
        return initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Admin))
               || initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Moderator))
               || toQuestion.UserId == initiator.Id;
    }

    private bool IsLengthValid(EditQuestionDto dto)
    {
        return dto.Title.HasMinLength(_businessRules.TitleMinLength)
               && dto.Body.HasMinLength(_businessRules.BodyMinLength)
               && dto.TagNames.Any();
    }

    private bool IsLengthValid(AskQuestionDto dto)
    {
        return dto.Title.HasMinLength(_businessRules.TitleMinLength)
               && dto.Body.HasMinLength(_businessRules.BodyMinLength)
               && dto.TagNames.Any();
    }
}