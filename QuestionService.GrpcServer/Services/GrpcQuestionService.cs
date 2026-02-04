using AutoMapper;
using Grpc.Core;
using QuestionService.Domain.Interfaces.Service;

namespace QuestionService.GrpcServer.Services;

public class GrpcQuestionService(
    IGetQuestionService questionService,
    IGetTagService tagService,
    IGetViewService viewService,
    IGetViewService voteService,
    IMapper mapper)
    : QuestionService.QuestionServiceBase
{
    public override async Task<GrpcQuestion> GetQuestionById(GetQuestionByIdRequest request, ServerCallContext context)
    {
        var questionResult = await questionService.GetByIdsAsync([request.Id], context.CancellationToken);
        if (!questionResult.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, questionResult.ErrorMessage!),
                new Metadata
                    { { nameof(questionResult.ErrorCode), questionResult.ErrorCode?.ToString() ?? string.Empty } });

        return mapper.Map<GrpcQuestion>(questionResult.Data.Single());
    }

    public override async Task<GetQuestionByIdsResponse> GetQuestionsByIds(GetQuestionByIdsRequest request,
        ServerCallContext context)
    {
        var questions = await questionService.GetByIdsAsync(request.Ids, context.CancellationToken);
        if (!questions.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, questions.ErrorMessage!),
                new Metadata { { nameof(questions.ErrorCode), questions.ErrorCode?.ToString() ?? string.Empty } });

        var response = new GetQuestionByIdsResponse();
        response.Questions.AddRange(questions.Data.Select(mapper.Map<GrpcQuestion>));

        return response;
    }
}