using AutoMapper;
using Grpc.Core;
using QuestionService.Application.Enum;
using QuestionService.Application.Resources;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Grpc;
using QuestionService.Grpc.Mappings;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Configurations.TestServices;

internal class GrpcTestUserService : UserService.UserServiceClient
{
    private static readonly IEnumerable<UserDto> Users = MockEntityProvidersGetters.GetUserDtos();

    private static readonly IMapper Mapper =
        new MapperConfiguration(cfg => cfg.AddMaps(typeof(GrpcMapping))).CreateMapper();

    public override GrpcUser GetUserWithRolesById(GetUserByIdRequest request, CallOptions options) =>
        GetUserById(request.UserId);

    public override GrpcUser GetUserWithRolesById(GetUserByIdRequest request, Metadata? headers = default,
        DateTime? deadline = null, CancellationToken cancellationToken = default) => GetUserById(request.UserId);

    public override AsyncUnaryCall<GrpcUser>
        GetUserWithRolesByIdAsync(GetUserByIdRequest request, CallOptions options) =>
        ToAsyncUnaryCall(GetUserById(request.UserId));

    public override AsyncUnaryCall<GrpcUser> GetUserWithRolesByIdAsync(GetUserByIdRequest request,
        Metadata headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default) =>
        ToAsyncUnaryCall(GetUserById(request.UserId));

    public override GetUsersByIdsResponse GetUsersByIds(GetUsersByIdsRequest request, CallOptions options) =>
        GetUsersByIds(request.UserIds);

    public override GetUsersByIdsResponse GetUsersByIds(GetUsersByIdsRequest request, Metadata headers = null,
        DateTime? deadline = null, CancellationToken cancellationToken = default) => GetUsersByIds(request.UserIds);

    public override AsyncUnaryCall<GetUsersByIdsResponse> GetUsersByIdsAsync(GetUsersByIdsRequest request,
        CallOptions options) => ToAsyncUnaryCall(GetUsersByIds(request.UserIds));

    public override AsyncUnaryCall<GetUsersByIdsResponse> GetUsersByIdsAsync(GetUsersByIdsRequest request,
        Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default) =>
        ToAsyncUnaryCall(GetUsersByIds(request.UserIds));

    private static AsyncUnaryCall<T> ToAsyncUnaryCall<T>(T response)
    {
        var responseTask = Task.FromResult(response);
        var metadataTask = Task.FromResult(new Metadata());

        return new AsyncUnaryCall<T>(
            responseTask,
            metadataTask,
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

    private static GetUsersByIdsResponse GetUsersByIds(IEnumerable<long> userIds)
    {
        var users = Users.Where(x => userIds.Contains(x.Id)).ToList();

        if (users.Count == 0)
            return userIds.Count() switch
            {
                <= 1 => throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.UserNotFound),
                    new Metadata { { "ErrorCode", ErrorCodes.UserNotFound.ToString() } }),
                >= 1 => throw new RpcException(new Status(StatusCode.InvalidArgument, ErrorMessage.UsersNotFound),
                    new Metadata
                    {
                        { "ErrorCode", "24" }
                    }) // We don't have ErrorCode for UsersNotFound because we don't use it in services
            };

        var grpcUsers = users.Select(Mapper.Map<GrpcUser>);

        var response = new GetUsersByIdsResponse();
        response.Users.AddRange(grpcUsers);

        return response;
    }
}