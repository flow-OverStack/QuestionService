using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl.Types;

public class TagType : ObjectType<Tag>
{
    protected override void Configure(IObjectTypeDescriptor<Tag> descriptor)
    {
        descriptor.Description("The tag type.");
        descriptor.Field(x => x.Name).Description("The name of the tag.");
        descriptor.Field(x => x.Description).Description("The description of the tag.");
        descriptor.Field(x => x.Questions).Description("The questions with this tag.");

        descriptor.Field(x => x.Questions)
            .ResolveWith<Resolvers>(x => x.GetQuestionsAsync(default!, default!));
    }

    private sealed class Resolvers
    {
        public async Task<IEnumerable<Question>> GetQuestionsAsync([Parent] Tag tag,
            [Service] IGetQuestionService questionService)
        {
            var result = await questionService.GetQuestionsWithTag(tag.Name);

            if (!result.IsSuccess)
                throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

            return result.Data;
        }
    }
}