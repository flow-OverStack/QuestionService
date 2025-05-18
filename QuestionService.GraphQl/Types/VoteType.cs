using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.ExtensionTypes;

namespace QuestionService.GraphQl.Types;

public class VoteType : ObjectType<Vote>
{
    protected override void Configure(IObjectTypeDescriptor<Vote> descriptor)
    {
        descriptor.Description("The vote type.");
        descriptor.Field(x => x.UserId).Description("The ID of the user that voted.");
        descriptor.Field(x => x.QuestionId).Description("The ID of the question that was voted.");
        descriptor.Field(x => x.ReputationChange).Description("The reputation change of the vote.");
        descriptor.Field(x => x.Question).Description("The question that was voted.");

        descriptor.Field(x => x.Question).ResolveWith<Resolvers>(x => x.GetQuestionAsync(default!, default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The voter.")
            .ResolveWith<Resolvers>(x => x.GetUserByVote(default!, default!))
            .Type<NonNullType<UserType>>();
    }

    private sealed class Resolvers
    {
        public async Task<Question> GetQuestionAsync([Parent] Vote vote, QuestionDataLoader questionLoader,
            CancellationToken cancellationToken)
        {
            var question = await questionLoader.LoadRequiredAsync(vote.QuestionId, cancellationToken);

            return question;
        }

        public UserDto GetUserByVote([Parent] Vote vote, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new UserDto { Id = vote.UserId };
        }
    }
}