using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.Domain.Entities;

public class Tag : IEntityId<long>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<Question> Questions { get; set; }

    public long Id { get; set; }
}