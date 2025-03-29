using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Services;
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
    public async Task<Question> GetQuestion(long id, [Service] IGetQuestionService questionService)
    {
        var result = await questionService.GetByIdAsync(id);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
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
    public async Task<Tag> GetTag(string name, [Service] IGetTagService tagService)
    {
        var result = await tagService.GetByNameAsync(name);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
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
    public async Task<Vote> GetVote(long questionId, long userId, [Service] IGetVoteService voteService)
    {
        var dto = new GetVoteDto(questionId, userId);
        var result = await voteService.GetByIdsAsync(dto);

        if (!result.IsSuccess)
            throw GraphQlExceptionHelper.GetException(result.ErrorMessage!);

        return result.Data;
    }
}