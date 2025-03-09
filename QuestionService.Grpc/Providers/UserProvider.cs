using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Settings;

namespace QuestionService.Grpc.Providers;

public class UserProvider(IOptions<GrpcHosts> grpcHosts, IMapper mapper) : IEntityProvider<UserDto>
{
    private readonly GrpcHosts _grpcHosts = grpcHosts.Value;

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        try
        {
            using var chanel = GrpcChannel.ForAddress(_grpcHosts.UsersHost);
            var client = new UserService.UserServiceClient(chanel);

            var user = await client.GetUserByIdAsync(new GetUserByIdRequest { UserId = id });
            return mapper.Map<UserDto>(user);
        }
        catch (RpcException e)
        {
            if (e.Status.Detail == ErrorMessage.UserNotFound)
                return default;
            throw;
        }
    }
}