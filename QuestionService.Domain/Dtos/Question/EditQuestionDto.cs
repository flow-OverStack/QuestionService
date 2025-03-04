namespace QuestionService.Domain.Dtos.Question;

public record EditQuestionDto(long Id, string Title, string Body, List<string> TagNames);