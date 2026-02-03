using GraphQL.Server.Ui.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QuestionService.Application.Settings;
using QuestionService.GraphQl.DataLoaders;
using QuestionService.GraphQl.ErrorFilters;
using QuestionService.GraphQl.Types;
using QuestionService.GraphQl.Types.Extension;
using QuestionService.GraphQl.Types.Sharable;

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
            .AddType<VoteTypeType>()
            .AddType<TagType>()
            .AddType<UserType>()
            .AddTypeExtension<CollectionSegmentInfoType>()
            .AddSorting()
            .AddFiltering()
            .AddErrorFilter<PublicErrorFilter>()
            .AddDataLoader<QuestionDataLoader>()
            .AddDataLoader<VoteDataLoader>()
            .AddDataLoader<VoteTypeDataLoader>()
            .AddDataLoader<TagDataLoader>()
            .AddDataLoader<ViewDataLoader>()
            .AddDataLoader<GroupTagDataLoader>()
            .AddDataLoader<GroupVoteDataLoader>()
            .AddDataLoader<GroupTagQuestionDataLoader>()
            .AddDataLoader<GroupViewDataLoader>()
            .AddDataLoader<GroupUserQuestionDataLoader>()
            .AddDataLoader<GroupUserViewDataLoader>()
            .AddDataLoader<GroupUserVoteDataLoader>()
            .AddDataLoader<GroupVoteTypeVoteDataLoader>()
            .AddApolloFederation(FederationVersion.Federation23)
            .ModifyPagingOptions(opt =>
            {
                using var provider = services.BuildServiceProvider();
                using var scope = provider.CreateAsyncScope();
                var defaultSize = scope.ServiceProvider.GetRequiredService<IOptions<ContentRules>>().Value
                    .DefaultPageSize;

                opt.DefaultPageSize = defaultSize;
                opt.IncludeTotalCount = true;
            })
            .AddDbContextCursorPagingProvider()
            .ModifyCostOptions(opt => opt.MaxFieldCost *= 3);
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