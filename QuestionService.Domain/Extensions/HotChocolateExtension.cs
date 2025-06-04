using HotChocolate.Language;
using QuestionService.Domain.Dtos.Request.Page;
using QuestionService.Domain.Enums;

namespace QuestionService.Domain.Extensions;

public static class HotChocolateExtension
{
    public static IEnumerable<OrderDto> ToDomainOrderBy(this ListValueNode? listValueNode)
    {
        if (listValueNode == null || listValueNode.Items.Count != 1) return [];

        var item = listValueNode.Items[0];

        if (item is not ObjectValueNode objectValueNode)
            throw new ArgumentException($"Item must be of type {nameof(ObjectValueNode)}.");

        return objectValueNode.Fields.Select(ParseOrderFromField).ToList();
    }

    private static OrderDto ParseOrderFromField(ObjectFieldNode field)
    {
        var columnName = field.Name.Value;

        if (field.Value is not EnumValueNode directionEnumNode)
            throw new ArgumentException($"The direction value for field '{columnName}' must be an enum (ASC or DESC).");

        var directionString = directionEnumNode.Value;

        if (!Enum.TryParse<SortDirection>(directionString, ignoreCase: true, out var direction))
            throw new ArgumentException($"Invalid sort direction '{directionString}' for field '{columnName}'.");

        return new OrderDto(columnName, direction);
    }
}