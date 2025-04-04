using Microsoft.EntityFrameworkCore.Storage;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces.Databases;

namespace QuestionService.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IStateSaveChanges
{
    IBaseRepository<Question> Questions { get; set; }

    IBaseRepository<Vote> Votes { get; set; }

    Task<IDbContextTransaction> BeginTransactionAsync();
}