namespace QuestionService.Domain.Dtos.Question;

public record AskQuestionDto(string Title, string Body, List<string> TagNames);