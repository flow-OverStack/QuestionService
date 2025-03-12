using Microsoft.EntityFrameworkCore.Storage;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;

namespace QuestionService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<Question> questions,
    IBaseRepository<QuestionTag> questionTags,
    IBaseRepository<Tag> tags)
    : IUnitOfWork
{
    public IBaseRepository<Question> Questions { get; set; } = questions;
    public IBaseRepository<QuestionTag> QuestionTags { get; set; } = questionTags;
    public IBaseRepository<Tag> Tags { get; set; } = tags;

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await context.Database.BeginTransactionAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}