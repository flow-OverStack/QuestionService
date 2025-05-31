using HotChocolate.Resolvers;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Request.Page;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Settings;

namespace QuestionService.GraphQl.Middlewares;

public class CursorPagingValidationMiddleware(FieldDelegate next)
{
    private const string BeforeArgName = "before";
    private const string AfterArgName = "after";
    private const string FirstArgName = "first";
    private const string LastArgName = "last";

    public async Task InvokeAsync(IMiddlewareContext context,
        INullSafeValidator<CursorPageDto> cursorPageValidator,
        IOptions<BusinessRules> businessRules)
    {
        if (context.Selection.Field.Arguments.Any(x =>
                x.Name is BeforeArgName or AfterArgName or FirstArgName or LastArgName))
        {
            var first = context.ArgumentValue<int?>(FirstArgName);
            var after = context.ArgumentValue<string?>(AfterArgName);
            var before = context.ArgumentValue<string?>(BeforeArgName);
            var last = context.ArgumentValue<int?>(LastArgName);

            // Specifying default values if need
            if (after == null && first == null && before == null && last == null)
                first = businessRules.Value.DefaultPageSize;
            if (after != null && first == null)
                first = businessRules.Value.DefaultPageSize;
            if (before != null && last == null)
                last = businessRules.Value.DefaultPageSize;

            var pagination = new CursorPageDto(first, after, before, last);

            if (!cursorPageValidator.IsValid(pagination, out var errors))
                throw GraphQlExceptionHelper.GetException(
                    $"{ErrorMessage.InvalidPagination}: {string.Join(' ', errors)}");
        }

        await next(context);
    }
}