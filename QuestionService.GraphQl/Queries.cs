using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
using QuestionService.GraphQl.DataLoaders;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl;

public class Queries()
{
    [GraphQLDescription("Returns a list of all questions")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Question>> GetQuestions([Service] IGetQuestionService questionService)
    {
        var result = await questionService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a question by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Question> GetQuestion(long id, QuestionDataLoader questionLoader)
    {
        //If the question is not found, data loader will throw GrpahQl exception
        var question = await questionLoader.LoadRequiredAsync(id);

        return question;
    }

    [GraphQLDescription("Returns a list of all tags")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Tag>> GetTags([Service] IGetTagService tagService)
    {
        var result = await tagService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a tag by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Tag> GetTag(string name, TagDataLoader tagLoader)
    {
        var tag = await tagLoader.LoadRequiredAsync(name);

        return tag;
    }

    [GraphQLDescription("Returns a list of all votes")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Vote>> GetVotes([Service] IGetVoteService voteService)
    {
        var result = await voteService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote by id of the question that was voted and the user that voted")]
    [UseFiltering]
    [UseSorting]
    public async Task<Vote> GetVote(long questionId, long userId, VoteDataLoader voteLoader)
    {
        var dto = new GetVoteDto(questionId, userId);
        var vote = await voteLoader.LoadRequiredAsync(dto);

        return vote;
    }

    [GraphQLDescription("Returns a list of all views")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<View>> GetViews([Service] IGetViewService viewService)
    {
        var result = await viewService.GetAllAsync();

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a view by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<View> GetView(long viewId, ViewDataLoader viewLoader)
    {
        var view = await viewLoader.LoadRequiredAsync(viewId);

        return view;
    }
}