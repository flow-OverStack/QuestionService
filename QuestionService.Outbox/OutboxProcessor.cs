using System.Reflection;
using Newtonsoft.Json;
using QuestionService.Outbox.Interfaces.Repository;
using QuestionService.Outbox.Interfaces.Service;
using QuestionService.Outbox.Interfaces.TopicProducer;
using QuestionService.Outbox.Messages;
using Serilog;

namespace QuestionService.Outbox;

public class OutboxProcessor(
    IOutboxRepository outboxRepository,
    ITopicProducerResolver producerResolver,
    ILogger logger)
    : IOutboxProcessor
{
    public async Task<int> ProcessOutboxMessagesAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        var messages = await outboxRepository.GetUnprocessedAsync(batchSize, cancellationToken);

        var tasks = messages.Select(x => ProduceOutboxMessagesAsync(x, cancellationToken));

        var results = await Task.WhenAll(tasks);

        var count = results.Count(x => x);

        return count;
    }

    private async Task<bool> ProduceOutboxMessagesAsync(OutboxMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var type = Assembly.GetExecutingAssembly().GetType(message.Type) ?? // FullName
                       Assembly.GetExecutingAssembly().GetTypes()
                           .First(x => x.Name == message.Type); // Name
            var content = JsonConvert.DeserializeObject(message.Content, type)!;

            var producer = producerResolver.GetProducerForType(type);

            await producer.ProduceAsync(content, cancellationToken);

            await outboxRepository.MarkAsProcessedAsync(message.Id, cancellationToken);

            logger.Information("Produced message: {content}. Type: {type}", message.Content, type);

            return true;
        }
        catch (Exception e)
        {
            await outboxRepository.MarkAsFailedAsync(message.Id, e.Message);

            logger.Error(e, "Failed to produce message: {errorMessage}.", e.Message);

            return false;
        }
    }
}