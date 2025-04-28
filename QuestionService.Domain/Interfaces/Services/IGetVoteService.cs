using QuestionService.Domain.Dtos.Vote;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Result;

namespace QuestionService.Domain.Interfaces.Services;

public interface IGetVoteService
{
    /// <summary>
    ///     Gets all votes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Vote>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets vote of questions by pairs of question id and user id
    /// </summary>
    /// <param name="dtos"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Vote>> GetByDtosAsync(IEnumerable<GetVoteDto> dtos,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets votes of questions by their ids
    /// </summary>
    /// <param name="questionIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetQuestionsVotesAsync(IEnumerable<long> questionIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets votes of users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Vote>>>> GetUsersVotesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}