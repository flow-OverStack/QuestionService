namespace QuestionService.Domain.Interfaces.GraphQlClients;

public interface IGraphQlClient<TEntity>
{
    /// <summary>
    /// Gets all of TEntity async via GraphQl
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    /// <summary>
    /// Gets one of TEntity by its id async via GraphQl
    /// </summary>
    /// <returns></returns>
    Task<TEntity?> GetByIdAsync(long id);
}