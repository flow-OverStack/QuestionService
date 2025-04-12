using HotChocolate.ApolloFederation.Types;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.GraphQl.Types;

namespace QuestionService.GraphQl.ExtensionTypes;

public class UserType : ObjectType<UserDto>
{
    protected override void Configure(IObjectTypeDescriptor<UserDto> descriptor)
    {
        descriptor.Name("User");
        descriptor.Description("The user type.");
        descriptor.ExtendServiceType();
        descriptor.Key(nameof(UserDto.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetUserById(default!));

        descriptor.Field("questions")
            .Description("The questions of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserQuestionsAsync(default!, default!))
            .Type<NonNullType<ListType<NonNullType<QuestionType>>>>();

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

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Question>> GetUserQuestionsAsync([Parent] UserDto user,
            [Service] IGetQuestionService questionService)
        {
            var result = await questionService.GetUserQuestions(user.Id);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }

        public static UserDto GetUserById(long id) => new() { Id = id };
    }
}