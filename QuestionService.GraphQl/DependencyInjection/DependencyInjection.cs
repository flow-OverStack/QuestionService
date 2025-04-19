using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.ErrorFilters;
using QuestionService.GraphQl.ExtensionTypes;
using QuestionService.GraphQl.Types;

namespace QuestionService.GraphQl.DependencyInjection;

public static class DependencyInjection
{
    private const string GraphQlEndpoint = "/graphql";
    private const string GraphQlVoyagerEndpoint = "/graphql-voyager";

    /// <summary>
    ///     Adds GraphQl services
    /// </summary>
    /// <param name="services"></param>
    public static void AddGraphQl(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddQueryType<Queries>()
            .AddType<QuestionType>()
            .AddType<VoteType>()
            .AddType<TagType>()
            .AddType<UserType>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<QuestionDataLoader>()
            .AddDataLoader<VoteDataLoader>()
            .AddDataLoader<TagDataLoader>()
            .AddDataLoader<ViewDataLoader>()
            .AddDataLoader<GroupTagDataLoader>()
            .AddDataLoader<GroupVoteDataLoader>()
            .AddDataLoader<GroupTagQuestionDataLoader>()
            .AddDataLoader<GroupViewDataLoader>()
            .AddDataLoader<GroupUserQuestionDataLoader>()
            .AddDataLoader<GroupUserViewDataLoader>()
            .AddDataLoader<GroupUserVoteDataLoader>()
            .AddApolloFederation();
    }

    /// <summary>
    ///     Enables the use of GraphQl services
    /// </summary>
    /// <param name="app"></param>
    public static void UseGraphQl(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.UseGraphQLVoyager(GraphQlVoyagerEndpoint, new VoyagerOptions { GraphQLEndPoint = GraphQlEndpoint });

        app.MapGraphQL();
    }
}