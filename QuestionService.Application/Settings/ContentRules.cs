namespace QuestionService.Application.Settings;

public class ContentRules
{
    public int TitleMinLength { get; set; }
    public int BodyMinLength { get; set; }
    public int UserViewSpamThreshold { get; set; }
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
}