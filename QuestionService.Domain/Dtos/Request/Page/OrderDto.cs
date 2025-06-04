using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Dtos.Request.Page;

public record OrderDto(string Field, SortDirection Direction);