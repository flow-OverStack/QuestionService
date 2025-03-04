using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuestionService.DAL.Result;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Dtos.Question;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.GraphQlClients;
using QuestionService.Domain.Interfaces.Repositories;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.Domain.Resources;

namespace QuestionService.Application.Services;

public class QuestionService(
    IBaseRepository<Question> questionRepository,
    IBaseRepository<Vote> voteRepository,
    IGraphQlClient<UserDto> userClient,
    IMapper mapper)
    : IQuestionService
{
    private const int TitleMinLength = 20;
    private const int BodyMinLength = 50;
    private const int MinReputationToUpvote = 15;
    private const int MinReputationToDownvote = 125;
    private const int DownvoteReputationChange = -1;
    private const int UpvoteReputationChange = 1;

    public async Task<BaseResult<QuestionDto>> AskQuestion(AskQuestionDto dto)
    {
        if (!IsDtoPropsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var user = await userClient.GetByIdAsync(dto.UserId);
        if (user == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var question = mapper.Map<Question>(dto);

        await questionRepository.CreateAsync(question);
        await questionRepository.SaveChangesAsync();

        var questionDto = mapper.Map<QuestionDto>(question);
        return BaseResult<QuestionDto>.Success(questionDto);
    }

    public async Task<BaseResult<QuestionDto>> EditQuestion(long initiatorId, EditQuestionDto dto)
    {
        if (!IsDtoPropsLengthValid(dto))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.LengthOutOfRange, (int)ErrorCodes.LengthOutOfRange);

        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await questionRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        question = mapper.Map<Question>(dto);

        questionRepository.Update(question);
        await questionRepository.SaveChangesAsync();

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<QuestionDto>> DeleteQuestion(long initiatorId, long questionId)
    {
        var question = await questionRepository.GetAll().FirstOrDefaultAsync(x => x.Id == questionId);
        var initiator = await userClient.GetByIdAsync(initiatorId);

        if (question == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator == null)
            return BaseResult<QuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (!HasAccess(initiator, question))
            return BaseResult<QuestionDto>.Failure(ErrorMessage.OperationForbidden, (int)ErrorCodes.OperationForbidden);

        questionRepository.Remove(question);
        await questionRepository.SaveChangesAsync();

        return BaseResult<QuestionDto>.Success(mapper.Map<QuestionDto>(question));
    }

    public async Task<BaseResult<VoteQuestionDto>> UpvoteQuestion(long initiatorId, long questionId)
    {
        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await questionRepository.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < MinReputationToUpvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation, (int)ErrorCodes.TooLowReputation);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);
        if (vote is { ReputationChange: >= UpvoteReputationChange })
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven, (int)ErrorCodes.VoteAlreadyGiven);

        vote!.ReputationChange = UpvoteReputationChange;
        voteRepository.Update(vote);
        await voteRepository.SaveChangesAsync();

        var dto = mapper.Map<VoteQuestionDto>(question);

        return BaseResult<VoteQuestionDto>.Success(dto);
    }

    public async Task<BaseResult<VoteQuestionDto>> DownvoteQuestion(long initiatorId, long questionId)
    {
        var initiator = await userClient.GetByIdAsync(initiatorId);
        var question = await questionRepository.GetAll()
            .Include(x => x.Votes)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (initiator == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        if (question == null)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.QuestionNotFound, (int)ErrorCodes.QuestionNotFound);

        if (initiator.Reputation < MinReputationToDownvote)
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.TooLowReputation, (int)ErrorCodes.TooLowReputation);

        var vote = question.Votes.FirstOrDefault(x => x.UserId == initiator.Id);
        if (vote is { ReputationChange: <= DownvoteReputationChange })
            return BaseResult<VoteQuestionDto>.Failure(ErrorMessage.VoteAlreadyGiven, (int)ErrorCodes.VoteAlreadyGiven);

        vote!.ReputationChange = DownvoteReputationChange;
        voteRepository.Update(vote);
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

    private static bool IsDtoPropsLengthValid(EditQuestionDto dto)
    {
        return StringHelper.HasMinimumLength(TitleMinLength, dto.Title)
               && StringHelper.HasMinimumLength(BodyMinLength, dto.Body)
               && dto.TagNames.Any();
    }

    private static bool IsDtoPropsLengthValid(AskQuestionDto dto)
    {
        return StringHelper.HasMinimumLength(TitleMinLength, dto.Title)
               && StringHelper.HasMinimumLength(BodyMinLength, dto.Body)
               && dto.TagNames.Any();
    }
}