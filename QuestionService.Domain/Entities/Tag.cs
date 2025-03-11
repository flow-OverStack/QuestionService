namespace QuestionService.Domain.Entities;

public class Tag
{
    public string Name { get; set; }

    public string Description { get; set; }

    public List<Question> Questions { get; set; }
}