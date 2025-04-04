using AutoMapper;
using Grpc.Core;
using QuestionService.Domain.Dtos.Entity;
using QuestionService.Domain.Interfaces.Providers;
using QuestionService.Domain.Resources;

namespace QuestionService.Grpc.Providers;

public class UserProvider(UserService.UserServiceClient client, IMapper mapper) : IEntityProvider<UserDto>
{
    public async Task<UserDto?> GetByIdAsync(long id)
    {
        try
        {
            var user = await client.GetUserByIdAsync(new GetUserByIdRequest { UserId = id });
            return mapper.Map<UserDto>(user);
        }
        catch (RpcException e) when (e.Status.Detail == ErrorMessage.UserNotFound)
        {
            return default;
        }
    }
}