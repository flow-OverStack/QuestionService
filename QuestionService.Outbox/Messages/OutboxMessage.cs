using QuestionService.Domain.Interfaces.Entity;
using QuestionService.Outbox.Enums;

namespace QuestionService.Outbox.Messages;

public class OutboxMessage : IEntityId<long>
{
    public string Type { get; set; }
    public string Content { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }

    public DateTime? NextRetryAt { get; set; }

    // We don't use a separate table for the status because OutboxMessage is not a domain entity
    public OutboxMessageStatus Status { get; set; }
    public long Id { get; set; }
}