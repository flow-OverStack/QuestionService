using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.Domain.Entities;

public class VoteType : IEntityId<long>
{
    public string Name { get; set; }

    public List<Vote> Votes { get; set; }
    public long Id { get; set; }
}