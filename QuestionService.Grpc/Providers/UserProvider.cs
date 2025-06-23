using AutoMapper;
using Grpc.Core;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Interfaces.Provider;
using QuestionService.Domain.Resources;

namespace QuestionService.Grpc.Providers;

public class UserProvider(UserService.UserServiceClient client, IMapper mapper) : IEntityProvider<UserDto>
{
    public async Task<UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await client.GetUserByIdAsync(new GetUserByIdRequest { UserId = id },
                cancellationToken: cancellationToken);
            return mapper.Map<UserDto>(user);
        }
        catch (RpcException e) when (e.Status.Detail == ErrorMessage.UserNotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<UserDto>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetUsersByIdsRequest();
            request.UserIds.AddRange(ids);

            var response = await client.GetUsersByIdsAsync(request, cancellationToken: cancellationToken);

            return response.Users.Select(mapper.Map<UserDto>);
        }
        catch (RpcException e) when (e.Status.Detail == ErrorMessage.UserNotFound ||
                                     e.Status.Detail == ErrorMessage.UsersNotFound)
        {
            return [];
        }
    }
}