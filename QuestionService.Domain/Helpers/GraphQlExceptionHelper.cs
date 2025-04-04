using HotChocolate;

namespace QuestionService.Domain.Helpers;

public static class GraphQlExceptionHelper
{
    public const string IsBusinessErrorExtension = "IsBusinessError";

    public static GraphQLException GetException(string errorMessage)
    {
        return new GraphQLException(ErrorBuilder.New()
            .SetMessage(errorMessage)
            .SetExtension(IsBusinessErrorExtension, true)
            .Build());
    }
}