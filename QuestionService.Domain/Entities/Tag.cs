using QuestionService.Domain.Interfaces.Entities;

namespace QuestionService.Domain.Entities;

public class Tag : IEntityId<long>
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public List<Question> Questions { get; set; }
}