using HotChocolate.ApolloFederation.Types;
using QuestionService.Domain.Dtos.ExternalEntity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Extensions;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Resources;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.Types.Extension;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.Types;

public class QuestionType : ObjectType<Question>
{
    protected override void Configure(IObjectTypeDescriptor<Question> descriptor)
    {
        descriptor.Description("The question type.");
        descriptor.Field(x => x.Id).Description("The ID of the question.");
        descriptor.Field(x => x.Title).Description("The tile of the question.");
        descriptor.Field(x => x.Body).Description("The body of the question.");
        descriptor.Field(x => x.UserId).Description("The ID of the author of the question.");
        descriptor.Field(x => x.Tags).Description("The tags of the question.");
        descriptor.Field(x => x.Votes).Description("The votes of the question.");
        descriptor.Field(x => x.CreatedAt).Description("Question creation time.");
        descriptor.Field(x => x.LastModifiedAt).Description("Question last modification time.");

        descriptor.Field(x => x.Tags).ResolveWith<Resolvers>(x => x.GetTagsAsync(default!, default!, default!));
        descriptor.Field(x => x.Votes).ResolveWith<Resolvers>(x => x.GetVotesAsync(default!, default!, default!));
        descriptor.Field(x => x.Views).ResolveWith<Resolvers>(x => x.GetViewsAsync(default!, default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The author of the question.")
            .ResolveWith<Resolvers>(x => x.GetUserByQuestion(default!, default!))
            .Type<NonNullType<UserType>>();


        descriptor.Key(nameof(Question.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetQuestionById(default!, default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Tag>> GetTagsAsync([Parent] Question question, GroupTagDataLoader tagLoader,
            CancellationToken cancellationToken)
        {
            var tags = await tagLoader.LoadRequiredAsync(question.Id, cancellationToken);

            // Have no tags is a business exception
            if (tags.Length == 0)
                throw GraphQlExceptionHelper.GetException(ErrorMessage.TagsNotFound);

            return tags;
        }

        public async Task<IEnumerable<Vote>> GetVotesAsync([Parent] Question question, GroupVoteDataLoader voteLoader,
            CancellationToken cancellationToken)
        {
            var votes = await voteLoader.LoadRequiredAsync(question.Id, cancellationToken);

            return votes;
        }

        public async Task<IEnumerable<View>> GetViewsAsync([Parent] Question question, GroupViewDataLoader viewLoader,
            CancellationToken cancellationToken)
        {
            var views = await viewLoader.LoadRequiredAsync(question.Id, cancellationToken);

            return views;
        }

        public static async Task<Question> GetQuestionById(long id, QuestionDataLoader questionLoader,
            CancellationToken cancellationToken)
        {
            var question = await questionLoader.LoadRequiredAsync(id, cancellationToken);

            return question;
        }

        public UserDto GetUserByQuestion([Parent] Question question, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new UserDto { Id = question.UserId };
        }
    }
}