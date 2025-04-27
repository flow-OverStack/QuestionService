using AutoMapper;
using Grpc.Core;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Enums;
using QuestionService.Domain.Resources;
using QuestionService.Grpc;
using QuestionService.Grpc.Mapping;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Configurations.TestServices;

internal class GrpcTestUserService : UserService.UserServiceClient
{
    private static readonly IEnumerable<UserDto> Users = MockEntityProvidersGetters.GetUserDtos();

    private static readonly IMapper Mapper =
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(GrpcMapping))).CreateMapper();

    public override GrpcUser GetUserById(GetUserByIdRequest request, CallOptions options)
    {
        return GetUserById(request.UserId);
    }

    public override GrpcUser GetUserById(GetUserByIdRequest request, Metadata headers = null, DateTime? deadline = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return GetUserById(request.UserId);
    }

    public override AsyncUnaryCall<GrpcUser> GetUserByIdAsync(GetUserByIdRequest request, Metadata headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var responseTask = Task.FromResult(GetUserById(request.UserId));
        var responseHeadersTask = Task.FromResult(new Metadata());

        return new AsyncUnaryCall<GrpcUser>(
            responseTask,
            responseHeadersTask,
            () => Status.DefaultSuccess,
            () => [],
            () => { });
    }

    public override AsyncUnaryCall<GrpcUser> GetUserByIdAsync(GetUserByIdRequest request, CallOptions options)
    {
        var responseTask = Task.FromResult(GetUserById(request.UserId));
        var responseHeadersTask = Task.FromResult(new Metadata());

        return new AsyncUnaryCall<GrpcUser>(
            responseTask,
            responseHeadersTask,
            () => Status.DefaultSuccess,
            () => [],
            () => { });
    }

    private static GrpcUser GetUserById(long id)
    {
        var user = Users.FirstOrDefault(x => x.Id == id);

        if (user == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.UserNotFound),
                new Metadata { { "ErrorCode", ErrorCodes.UserNotFound.ToString() } });

        return Mapper.Map<GrpcUser>(user);
    }
}