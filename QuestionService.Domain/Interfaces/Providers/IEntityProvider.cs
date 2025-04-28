namespace QuestionService.Domain.Interfaces.Providers;

public interface IEntityProvider<TEntity>
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}