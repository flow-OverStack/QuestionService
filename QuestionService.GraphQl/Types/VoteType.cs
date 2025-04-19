using QuestionService.Domain.Entities;
using QuestionService.GraphQl.DataLoaders;

namespace QuestionService.GraphQl.Types;

public class VoteType : ObjectType<Vote>
{
    protected override void Configure(IObjectTypeDescriptor<Vote> descriptor)
    {
        descriptor.Description("The vote type.");
        descriptor.Field(x => x.UserId).Description("The id of the user that voted.");
        descriptor.Field(x => x.QuestionId).Description("The id of the question that was voted.");
        descriptor.Field(x => x.ReputationChange).Description("The reputation change of the vote.");
        descriptor.Field(x => x.Question).Description("The question that was voted.");

        descriptor.Field(x => x.Question).ResolveWith<Resolvers>(x => x.GetQuestionAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<Question> GetQuestionAsync([Parent] Vote vote, QuestionDataLoader questionLoader)
        {
            var question = await questionLoader.LoadRequiredAsync(vote.QuestionId);

            return question;
        }
    }
}