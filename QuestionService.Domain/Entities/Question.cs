using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.Domain.Entities;

public class Question : IEntityId<long>, IAuditable
{
    public string Title { get; set; }

    public string Body { get; set; }

    public long UserId { get; set; }

    public List<Tag> Tags { get; set; }

    public List<Vote> Votes { get; set; }

    public List<View> Views { get; set; }
    public bool Enabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
    public long Id { get; set; }
}