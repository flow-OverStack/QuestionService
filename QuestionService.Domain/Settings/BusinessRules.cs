namespace QuestionService.Domain.Settings;

public class BusinessRules
{
    public int TitleMinLength { get; set; }
    public int BodyMinLength { get; set; }
    public int MinReputationToUpvote { get; set; }
    public int MinReputationToDownvote { get; set; }
    public int DownvoteReputationChange { get; set; }
    public int UpvoteReputationChange { get; set; }
}