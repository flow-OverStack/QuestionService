using QuestionService.Domain.Results;

namespace QuestionService.Outbox.Interfaces.Service;

public interface IOutboxResetService
{
    /// <summary>
    ///     Resets outbox messages that have been sent successfully and are older than a certain threshold
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult> ResetOutboxMessagesAsync(CancellationToken cancellationToken = default);
}