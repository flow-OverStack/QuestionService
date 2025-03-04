namespace QuestionService.Domain.Dtos.Question;

public record QuestionDto(long Id, string Title, string Body, List<string> TagNames, long UserId);