using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Api.Persistence;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _set;

    public EfRepository(DbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _set.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();

    public async Task AddAsync(T entity) => await _set.AddAsync(entity);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}
