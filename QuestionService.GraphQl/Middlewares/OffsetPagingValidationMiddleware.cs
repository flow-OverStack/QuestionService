using HotChocolate.Resolvers;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Request.Page;
using QuestionService.Domain.Helpers;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.Domain.Resources;
using QuestionService.Domain.Settings;

namespace QuestionService.GraphQl.Middlewares;

public class OffsetPagingValidationMiddleware(FieldDelegate next)
{
    private const string SkipArgName = "skip";
    private const string TakeArgName = "take";

    public async Task InvokeAsync(IMiddlewareContext context,
        INullSafeValidator<OffsetPageDto> offsetPageValidator,
        INullSafeValidator<CursorPageDto> cursorPageValidator,
        IOptions<BusinessRules> businessRules)
    {
        if (context.Selection.Field.Arguments.Any(x => x.Name is SkipArgName or TakeArgName))
        {
            var skip = context.ArgumentValue<int?>(SkipArgName) ?? 0; // Value by default
            var take = context.ArgumentValue<int?>(TakeArgName) ??
                       businessRules.Value.DefaultPageSize; // Value by default

            var pagination = new OffsetPageDto(skip, take);

            if (!offsetPageValidator.IsValid(pagination, out var errors))
                throw GraphQlExceptionHelper.GetException(
                    $"{ErrorMessage.InvalidPagination}: {string.Join(' ', errors)}");
        }

        await next(context);
    }
}