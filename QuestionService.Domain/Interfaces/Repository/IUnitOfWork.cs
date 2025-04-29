using Microsoft.EntityFrameworkCore.Storage;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Database;

namespace QuestionService.Domain.Interfaces.Repository;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<Question> Questions { get; set; }

    IBaseRepository<Vote> Votes { get; set; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}