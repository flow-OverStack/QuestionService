namespace QuestionService.Application.Settings;

public class BusinessRules
{
    public int TitleMinLength { get; set; }
    public int BodyMinLength { get; set; }
    public int MinReputationToUpvote { get; set; }
    public int MinReputationToDownvote { get; set; }
    public int DownvoteReputationChange { get; set; }
    public int UpvoteReputationChange { get; set; }
    public int UserViewSpamThreshold { get; set; }
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}