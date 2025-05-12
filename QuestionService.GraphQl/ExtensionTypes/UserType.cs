using HotChocolate.ApolloFederation.Types;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.GraphQl.DataLoaders;
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
            .ResolveReferenceWith(_ => Resolvers.GetUserById(default!, default!));

        descriptor.Field("questions")
            .Description("The questions of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserQuestionsAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<QuestionType>>>>();

        descriptor.Field("views")
            .Description("The views of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserViewsAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<ViewType>>>>();

        descriptor.Field("votes")
            .Description("The votes of the user.")
            .ResolveWith<Resolvers>(x => x.GetUserVotesAsync(default!, default!, default!))
            .Type<NonNullType<ListType<NonNullType<VoteType>>>>();

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
            GroupUserQuestionDataLoader questionLoader, CancellationToken cancellationToken)
        {
            var questions = await questionLoader.LoadRequiredAsync(user.Id, cancellationToken);

            return questions;
        }

        public async Task<IEnumerable<View>> GetUserViewsAsync([Parent] UserDto user,
            GroupUserViewDataLoader viewLoader, CancellationToken cancellationToken)
        {
            var views = await viewLoader.LoadRequiredAsync(user.Id, cancellationToken);

            return views;
        }

        public async Task<IEnumerable<Vote>> GetUserVotesAsync([Parent] UserDto user,
            GroupUserVoteDataLoader voteLoader, CancellationToken cancellationToken)
        {
            var views = await voteLoader.LoadRequiredAsync(user.Id, cancellationToken);

            return views;
        }

        public static UserDto GetUserById(long id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new UserDto { Id = id };
        }
    }
}