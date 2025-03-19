using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestionService.DAL;
using QuestionService.Domain.Entities;
using QuestionService.Tests.Configurations;

namespace QuestionService.Tests.FunctionalTests.Configurations;

internal static class PrepDb
{
    public static void PrepPopulation(this IServiceScope serviceScope)
    {
        var questions = MockRepositoriesGetters.GetQuestions().Select(x => new Question
        {
            Title = x.Title,
            Body = x.Body,
            Views = x.Views,
            UserId = x.UserId,
            Reputation = x.Reputation,
            CreatedAt = x.CreatedAt,
            LastModifiedAt = x.LastModifiedAt
        });

        var tags = MockRepositoriesGetters.GetTags();
        var questionTags = MockRepositoriesGetters.GetQuestionTags();
        var votes = MockRepositoriesGetters.GetVotes();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.Migrate();

        dbContext.Set<Question>().AddRange(questions);
        dbContext.Set<Tag>().AddRange(tags);
        dbContext.Set<QuestionTag>().AddRange(questionTags);
        dbContext.Set<Vote>().AddRange(votes);

        dbContext.SaveChanges();
    }
}