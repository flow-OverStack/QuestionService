using QuestionService.Domain.Entities;
using QuestionService.GraphQl.DataLoaders;

namespace QuestionService.GraphQl.Types;

public class ViewType : ObjectType<View>
{
    protected override void Configure(IObjectTypeDescriptor<View> descriptor)
    {
        descriptor.Description("The view type.");
        descriptor.Field(x => x.Id).Description("The id of the view.");
        descriptor.Field(x => x.QuestionId).Description("The id of the question that was viewed.");
        descriptor.Field(x => x.UserId).Description("The id of the viewer.");
        descriptor.Field(x => x.UserIp).Description("The IP address of the viewer.");
        descriptor.Field(x => x.UserFingerprint).Description("The unique fingerprint of the viewer's device.");

        descriptor.Field(x => x.Question).ResolveWith<Resolvers>(x => x.GetQuestionAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<Question> GetQuestionAsync([Parent] View view, QuestionDataLoader questionLoader)
        {
            var question = await questionLoader.LoadRequiredAsync(view.QuestionId);

            return question;
        }
    }
}