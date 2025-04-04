using QuestionService.Domain.Helpers;
using QuestionService.Domain.Resources;

namespace QuestionService.GraphQl.ErrorFilters;

public class PublicErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Extensions != null
            && error.Extensions.TryGetValue(GraphQlExceptionHelper.IsBusinessErrorExtension, out var value)
            && value is true)
            return error.RemoveExtension(GraphQlExceptionHelper.IsBusinessErrorExtension).WithMessage(error.Message);

        return error.WithMessage($"{ErrorMessage.InternalServerError}: {error.Exception?.Message ?? error.Message}");
    }
}