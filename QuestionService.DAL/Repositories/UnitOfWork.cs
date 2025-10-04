using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Database;
using QuestionService.Domain.Interfaces.Repository;

namespace QuestionService.DAL.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IBaseRepository<Question> questions,
    IBaseRepository<Vote> votes)
    : IUnitOfWork
{
    public IBaseRepository<Question> Questions { get; set; } = questions;

    public IBaseRepository<Vote> Votes { get; set; } = votes;

    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new DbContextTransaction(transaction);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}