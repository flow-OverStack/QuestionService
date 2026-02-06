using System.Reflection;
using System.Runtime.CompilerServices;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.Options;
using QuestionService.Application.Resources;
using QuestionService.Application.Settings;
using QuestionService.Domain.Dtos.Page;
using QuestionService.Domain.Interfaces.Validation;
using QuestionService.GraphQl.Helpers;

namespace QuestionService.GraphQl.Middlewares;

public class OffsetPagingValidationMiddleware(FieldDelegate next)
{
    private const string SkipArgName = "skip";
    private const string TakeArgName = "take";

    public async Task InvokeAsync(IMiddlewareContext context,
        INullSafeValidator<OffsetPageDto> offsetPageValidator,
        IOptions<PaginationRules> paginationRules)
    {
        var skip = context.ArgumentValue<int?>(SkipArgName) ?? 0; // Value by default
        var take = context.ArgumentValue<int?>(TakeArgName) ??
                   paginationRules.Value.DefaultPageSize; // Value by default

        var pagination = new OffsetPageDto(skip, take);

        if (!offsetPageValidator.IsValid(pagination, out var errors))
            throw GraphQlExceptionHelper.GetException(
                $"{ErrorMessage.InvalidPagination}: {string.Join(' ', errors)}");

        await next(context);
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class UseOffsetPagingValidationMiddlewareAttribute : ObjectFieldDescriptorAttribute
{
    public UseOffsetPagingValidationMiddlewareAttribute([CallerLineNumber] int order = 0)
    {
        Order = order;
    }

    protected override void OnConfigure(IDescriptorContext context,
        IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Use<OffsetPagingValidationMiddleware>();
    }
}