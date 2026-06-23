namespace Infrastructure.Api.Persistence;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);

    Task<IEnumerable<T>> GetAllAsync();

    Task AddAsync(T entity);

    void Update(T entity);

    void Remove(T entity);

    Task<int> SaveChangesAsync();
}
