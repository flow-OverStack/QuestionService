using AutoMapper;
using QuestionService.Domain.Dtos.GraphQl;
using QuestionService.Domain.Exceptions.GraphQl;
using QuestionService.Domain.Interfaces.GraphQlClients;
using StrawberryShake;

namespace QuestionService.GraphQlClient.Clients.UserClient;

public class GraphQlUserClient(GraphQlClient.UserClient userClient, IMapper mapper) : IGraphQlClient<UserDto>
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await userClient.GetAllUsers.ExecuteAsync();
        if (users.IsErrorResult())
            throw new GraphQlFetchException(users.Errors);

        if (users.Data is { Users.Count: 0 })
            return Array.Empty<UserDto>();

        var userDtos = users.Data!.Users.Select(mapper.Map<UserDto>);

        return userDtos;
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        var users = await userClient.GetUserById.ExecuteAsync(id);

        if (users.IsErrorResult())
            throw new GraphQlFetchException(users.Errors);


        if (users.Data is { Users.Count: 0 })
            return null;


        return mapper.Map<UserDto>(users.Data!.Users[0]); //First and single user
    }
}