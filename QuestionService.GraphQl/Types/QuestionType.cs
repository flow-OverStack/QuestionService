using HotChocolate.ApolloFederation.Types;
using QuestionService.Domain.Dtos.Entity;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.ExtensionTypes;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.Types;

public class QuestionType : ObjectType<Question>
{
    protected override void Configure(IObjectTypeDescriptor<Question> descriptor)
    {
        descriptor.Description("The question type.");
        descriptor.Field(x => x.Id).Description("The ID of the user.");
        descriptor.Field(x => x.Title).Description("The tile of the question.");
        descriptor.Field(x => x.Body).Description("The body of the question.");
        descriptor.Field(x => x.Views).Description("Count of views of the question.");
        descriptor.Field(x => x.UserId).Description("The ID of the author of the question.");
        descriptor.Field(x => x.Reputation).Description("The reputation of the question.");
        descriptor.Field(x => x.Tags).Description("The tags of the question.");
        descriptor.Field(x => x.Votes).Description("The votes of the question.");
        descriptor.Field(x => x.CreatedAt).Description("Question creation time.");
        descriptor.Field(x => x.LastModifiedAt).Description("Question last modification time.");

        descriptor.Field(x => x.Tags).ResolveWith<Resolvers>(x => x.GetTagsAsync(default!, default!));
        descriptor.Field(x => x.Votes).ResolveWith<Resolvers>(x => x.GetVotesAsync(default!, default!));

        descriptor.Field("user") // Field for user from UserService
            .Description("The author of the question")
            .ResolveWith<Resolvers>(x => x.GetUserByQuestion(default!))
            .Type<UserType>();


        descriptor.Key(nameof(Question.Id).LowercaseFirstLetter())
            .ResolveReferenceWith(_ => Resolvers.GetQuestionById(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Tag>> GetTagsAsync([Parent] Question question,
            [Service] IGetTagService tagService)
        {
            var result = await tagService.GetQuestionTags(question.Id);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }

        public async Task<IEnumerable<Vote>> GetVotesAsync([Parent] Question question,
            [Service] IGetVoteService voteService)
        {
            var result = await voteService.GetQuestionVotesAsync(question.Id);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }

        public static async Task<Question> GetQuestionById(long id, QuestionDataLoader questionLoader)
        {
            var question = await questionLoader.LoadRequiredAsync(id);

            return question;
        }

        public UserDto GetUserByQuestion([Parent] Question question) => new() { Id = question.UserId };
    }
}