namespace QuestionService.Domain.Entities;

public class Vote
{
    public long UserId { get; set; }
    public long QuestionId { get; set; }

    public Question Question { get; set; }
    public int ReputationChange { get; set; }
}