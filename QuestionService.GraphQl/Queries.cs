using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Service;
using QuestionService.GraphQl.DataLoaders;
using Tag = QuestionService.Domain.Entities.Tag;

namespace QuestionService.GraphQl;

public class Queries
{
    [GraphQLDescription("Returns a list of all questions")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Question>> GetQuestions([Service] IGetQuestionService questionService,
        CancellationToken cancellationToken)
    {
        var result = await questionService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a question by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Question?> GetQuestion(long id, QuestionDataLoader questionLoader,
        CancellationToken cancellationToken)
    {
        var question = await questionLoader.LoadAsync(id, cancellationToken);

        return question;
    }

    [GraphQLDescription("Returns a list of all tags")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Tag>> GetTags([Service] IGetTagService tagService,
        CancellationToken cancellationToken)
    {
        var result = await tagService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a tag by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<Tag?> GetTag(long id, TagDataLoader tagLoader, CancellationToken cancellationToken)
    {
        var tag = await tagLoader.LoadAsync(id, cancellationToken);

        return tag;
    }

    [GraphQLDescription("Returns a list of all votes")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<Vote>> GetVotes([Service] IGetVoteService voteService,
        CancellationToken cancellationToken)
    {
        var result = await voteService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a vote by id of the question that was voted and the user that voted")]
    [UseFiltering]
    [UseSorting]
    public async Task<Vote?> GetVote(long questionId, long userId, VoteDataLoader voteLoader,
        CancellationToken cancellationToken)
    {
        var dto = new GetVoteDto(questionId, userId);
        var vote = await voteLoader.LoadAsync(dto, cancellationToken);

        return vote;
    }

    [GraphQLDescription("Returns a list of all views")]
    [UseFiltering]
    [UseSorting]
    public async Task<IEnumerable<View>> GetViews([Service] IGetViewService viewService,
        CancellationToken cancellationToken)
    {
        var result = await viewService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }

    [GraphQLDescription("Returns a view by its id")]
    [UseFiltering]
    [UseSorting]
    public async Task<View?> GetView(long id, ViewDataLoader viewLoader, CancellationToken cancellationToken)
    {
        var view = await viewLoader.LoadAsync(id, cancellationToken);

        return view;
    }
}