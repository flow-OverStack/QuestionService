namespace QuestionService.Domain.Interfaces.Provider;

public interface IEntityProvider<TEntity>
{
    Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}