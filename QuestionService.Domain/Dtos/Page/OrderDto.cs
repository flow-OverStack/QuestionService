using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Dtos.Page;

public record OrderDto(string Field, SortDirection Direction);