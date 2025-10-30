using QuestionService.Domain.Interfaces.Entity;

namespace QuestionService.Domain.Entities;

public class VoteType : IEntityId<long>
{
    public string Name { get; set; }

    // Question reputation is calculated as the sum of ReputationChange of all votes
    public int ReputationChange { get; set; }

    public List<Vote> Votes { get; set; }
    public long Id { get; set; }
}