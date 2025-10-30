using QuestionService.Domain.Entities;
using QuestionService.GraphQl.DataLoaders;

namespace QuestionService.GraphQl.Types;

public class VoteTypeType : ObjectType<Domain.Entities.VoteType>
{
    protected override void Configure(IObjectTypeDescriptor<Domain.Entities.VoteType> descriptor)
    {
        descriptor.Description("Represents the type of a vote.");

        descriptor.Field(x => x.Id).Description("The unique identifier of the vote type.");
        descriptor.Field(x => x.Name).Description("The name of the vote type.");
        descriptor.Field(x => x.Votes).Description("The votes associated with this vote type.");
        descriptor.Field(x => x.Votes).Description("A list of votes of this type.");

        descriptor.Field(x => x.Votes).ResolveWith<Resolvers>(x => x.GetVotesAsync(default!, default!, default));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Vote>> GetVotesAsync([Parent] Domain.Entities.VoteType voteType,
            GroupVoteTypeVoteDataLoader voteLoader, CancellationToken cancellationToken)
        {
            var votes = await voteLoader.LoadRequiredAsync(voteType.Id, cancellationToken);

            return votes;
        }
    }
}