namespace QuestionService.Outbox.Enums;

public enum OutboxMessageStatus
{
    Pending = 0,
    Failed = 1,
    Processed = 2,
    Dead = 3
}