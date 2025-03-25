namespace QuestionService.Outbox.Interfaces.Services;

public interface IOutboxService
{
    /// <summary>
    ///     Adds T to outbox
    /// </summary>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task AddToOutboxAsync<T>(T message);
}