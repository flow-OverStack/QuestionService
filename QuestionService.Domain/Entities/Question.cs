using QuestionService.Domain.Interfaces.Entities;

namespace QuestionService.Domain.Entities;

public class Question : IEntityId<long>, IAuditable
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }
    
    public int Views { get; set; }
    
    public long UserId { get; set; }
    
    public int Reputation { get; set; }
    
    public List<Tag> Tags { get; set; }
}