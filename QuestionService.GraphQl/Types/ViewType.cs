using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.Types.Extension;

namespace QuestionService.GraphQl.Types;

public class ViewType : ObjectType<View>
{
    protected override void Configure(IObjectTypeDescriptor<View> descriptor)
    {
        descriptor.Description("The view type.");
        descriptor.Field(x => x.Id).Description("The ID of the view.");
        descriptor.Field(x => x.QuestionId).Description("The ID of the question that was viewed.");
        descriptor.Field(x => x.UserId).Description("The ID of the viewer.");
        descriptor.Field(x => x.UserIp).Description("The IP address of the viewer.");
        descriptor.Field(x => x.UserFingerprint).Description("The unique fingerprint of the viewer's device.");

        descriptor.Field(x => x.Question).ResolveWith<Resolvers>(x => x.GetQuestionAsync(default!, default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The viewer.")
            .ResolveWith<Resolvers>(x => x.GetUserByView(default!, default!))
            .Type<UserType>(); // Can be null
    }

    private sealed class Resolvers
    {
        public async Task<Question> GetQuestionAsync([Parent] View view, QuestionDataLoader questionLoader,
            CancellationToken cancellationToken)
        {
            var question = await questionLoader.LoadRequiredAsync(view.QuestionId, cancellationToken);

            return question;
        }

        public UserDto? GetUserByView([Parent] View view, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return view.UserId != null ? new UserDto { Id = (long)view.UserId } : null;
        }
    }
}