using QuestionService.Domain.Interfaces.Entities;

namespace QuestionService.Domain.Entities;

public class View : IEntityId<long>
{
    public long QuestionId { get; set; }

    public long? UserId { get; set; }

    public string? UserIp { get; set; }

    public string? UserFingerprint { get; set; }

    public Question Question { get; set; }
    public long Id { get; set; }
}