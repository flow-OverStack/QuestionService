using Microsoft.EntityFrameworkCore.Storage;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Repositories;

namespace QuestionService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<Question> questions,
    IBaseRepository<Vote> votes)
    : IUnitOfWork
{
    public IBaseRepository<Question> Questions { get; set; } = questions;

    public IBaseRepository<Vote> Votes { get; set; } = votes;

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}