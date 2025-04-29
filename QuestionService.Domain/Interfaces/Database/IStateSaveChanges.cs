namespace QuestionService.Domain.Interfaces.Database;

public interface IStateSaveChanges
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}