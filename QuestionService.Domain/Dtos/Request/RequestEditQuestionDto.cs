namespace QuestionService.Domain.Dtos.Request;

public record RequestEditQuestionDto(string Title, string Body, IEnumerable<string> TagNames);