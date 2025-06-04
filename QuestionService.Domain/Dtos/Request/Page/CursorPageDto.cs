namespace QuestionService.Domain.Dtos.Request.Page;

public record CursorPageDto(int? First, string? After, string? Before, int? Last, IEnumerable<OrderDto>? Order);