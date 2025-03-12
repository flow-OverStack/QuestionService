using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QuestionService.DAL.Result;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Settings;

namespace QuestionService.Application.Services;

public class QuestionService(
    IUnitOfWork unitOfWork,
    IBaseRepository<Vote> voteRepository,
    IEntityProvider<UserDto> userClient,
    IOptions<BusinessRules> businessRules,
    IMapper mapper)
    : IQuestionService
{
    private readonly BusinessRules _businessRules = businessRules.Value;

    public async Task<BaseResult<QuestionDto>> AskQuestion(long initiatorId, AskQuestionDto dto)
    {
        if (!IsDtoPropsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var user = await userClient.GetByIdAsync(initiatorId);
        if (user == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var tags = await unitOfWork.Tags.GetAll().Where(x => dto.TagNames.Contains(x.Name)).ToListAsync();
        if (tags.Count != dto.TagNames.Count)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        var question = mapper.Map<Question>(dto);
        question.UserId = initiatorId;
        question.Tags = tags;

        await unitOfWork.Questions.CreateAsync(question);
        await unitOfWork.Questions.SaveChangesAsync();

        var questionDto = mapper.Map<QuestionDto>(question);
        return BaseResult<QuestionDto>.Success(questionDto);
    }

    public async Task<BaseResult<QuestionDto>> EditQuestion(long initiatorId, EditQuestionDto dto)
    {
        if (!IsDtoPropsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        var tags = await unitOfWork.Tags.GetAll().Where(x => dto.TagNames.Contains(x.Name)).ToListAsync();
        if (tags.Count != dto.TagNames.Count)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.TagsNotFound, (int)ErrorCodes.TagsNotFound);

        mapper.Map(dto, question);
        question.Tags = tags;

        unitOfWork.Questions.Update(question);
        await unitOfWork.Questions.SaveChangesAsync();

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<QuestionDto>> DeleteQuestion(long initiatorId, long questionId)
    {
        var question = await unitOfWork.Questions.GetAll().FirstOrDefaultAsync(x => x.Id == questionId);
        var initiator = await userClient.GetByIdAsync(initiatorId);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        unitOfWork.Questions.Remove(question);
        await unitOfWork.Questions.SaveChangesAsync();

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<VoteQuestionDto>> UpvoteQuestion(long initiatorId, long questionId)
    {
        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < _businessRules.MinReputationToUpvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        if (vote == null)
        {
            vote = new Vote
            {
                QuestionId = question.Id,
                UserId = initiator.Id
            };

            await voteRepository.CreateAsync(vote);
        }
        else
        {
            if (vote.ReputationChange >= _businessRules.UpvoteReputationChange)
                return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                    (int)ErrorCodes.VoteAlreadyGiven);

            vote.ReputationChange = _businessRules.UpvoteReputationChange;
            voteRepository.Update(vote);
        }

        await voteRepository.SaveChangesAsync();

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    public async Task<BaseResult<VoteQuestionDto>> DownvoteQuestion(long initiatorId, long questionId)
    {
        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await unitOfWork.Questions.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < _businessRules.MinReputationToDownvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation,
                (int)ErrorCodes.OperationForbidden);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);

        if (vote == null)
        {
            vote = new Vote
            {
                QuestionId = question.Id,
                UserId = initiator.Id
            };

            await voteRepository.CreateAsync(vote);
        }
        else
        {
            if (vote.ReputationChange <= _businessRules.DownvoteReputationChange)
                return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven,
                    (int)ErrorCodes.VoteAlreadyGiven);

            vote.ReputationChange = _businessRules.DownvoteReputationChange;
            voteRepository.Update(vote);
        }

        await voteRepository.SaveChangesAsync();

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    private static bool HasAccess(UserDto initiator, Question toQuestion)
    {
        return initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Admin))
               || initiator.Roles.Select(x => x.Name).Contains(nameof(Roles.Moderator))
               || toQuestion.UserId == initiator.Id;
    }

    private bool IsDtoPropsLengthValid(EditQuestionDto dto)
    {
        return StringHelper.HasMinimumLength(_businessRules.TitleMinLength, dto.Title)
               && StringHelper.HasMinimumLength(_businessRules.BodyMinLength, dto.Body)
               && dto.TagNames.Any();
    }

    private bool IsDtoPropsLengthValid(AskQuestionDto dto)
    {
        return StringHelper.HasMinimumLength(_businessRules.TitleMinLength, dto.Title)
               && StringHelper.HasMinimumLength(_businessRules.BodyMinLength, dto.Body)
               && dto.TagNames.Any();
    }
}