namespace QuestionService.Domain.Dtos.Question;

public record AskQuestionDto(string Title, string Body, IEnumerable<string> TagNames);