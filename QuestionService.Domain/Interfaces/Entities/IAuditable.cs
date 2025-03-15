namespace QuestionService.Domain.Interfaces.Entities;

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }

    public DateTime? LastModifiedAt { get; set; }
}