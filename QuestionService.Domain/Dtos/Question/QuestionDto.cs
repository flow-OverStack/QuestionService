namespace QuestionService.Domain.Dtos.Question;

public record QuestionDto(long Id, string Title, string Body, string[] TagNames, long UserId);