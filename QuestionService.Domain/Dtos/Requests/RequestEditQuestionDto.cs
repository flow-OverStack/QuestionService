namespace QuestionService.Domain.Dtos.Requests;

public record RequestEditQuestionDto(string Title, string Body, List<string> TagNames);