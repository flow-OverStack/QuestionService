namespace QuestionService.Outbox.Interfaces.Services;

public interface IOutboxService
{
    /// <summary>
    ///     Adds T to outbox
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task AddToOutboxAsync<T>(T message, CancellationToken cancellationToken = default);
}