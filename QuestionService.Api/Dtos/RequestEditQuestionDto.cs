namespace QuestionService.Api.Dtos;

public record RequestEditQuestionDto(string Title, string Body, IEnumerable<string> TagNames);