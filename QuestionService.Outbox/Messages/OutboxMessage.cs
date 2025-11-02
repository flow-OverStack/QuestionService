using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.Outbox.Messages;

public class OutboxMessage : IEntityId<long>
{
    public string Type { get; set; }
    public string Content { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public long Id { get; set; }
}