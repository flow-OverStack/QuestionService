namespace QuestionService.Domain.Entities;

public class Vote
{
    public long UserId { get; set; }

    public long QuestionId { get; set; }
    public Question Question { get; set; }

    public long VoteTypeId { get; set; }
    public VoteType VoteType { get; set; }
}