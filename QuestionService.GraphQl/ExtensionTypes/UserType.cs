using HotChocolate.ApolloFederation.Types;
using QuestionService.Domain.Dtos.Entity;
using QuestionService.Domain.Helpers;

namespace QuestionService.GraphQl.ExtensionTypes;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        descriptor.Description("The user type.");
        descriptor.ExtendServiceType();
        descriptor.Key(nameof(UserDto.Id).LowercaseFirstLetter());

        //Ignore fields that will be retrieved from UserService
        descriptor.Field(x => x.KeycloakId).Ignore();
        descriptor.Field(x => x.Username).Ignore();
        descriptor.Field(x => x.Email).Ignore();
        descriptor.Field(x => x.LastLoginAt).Ignore();
        descriptor.Field(x => x.Reputation).Ignore();
        descriptor.Field(x => x.Roles).Ignore();
        descriptor.Field(x => x.CreatedAt).Ignore();
        descriptor.Field(x => x.ReputationEarnedToday).Ignore();
    }
}