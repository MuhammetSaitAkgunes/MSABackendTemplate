using Microsoft.EntityFrameworkCore;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Domain.Common;
using MSABackendTemplate.Persistence.Contexts;

namespace MSABackendTemplate.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Updated to match interface: Task (not Task<T>)
    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
    }

    // Updated to match interface: DeleteAsync(Guid id)
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Set<T>().FindAsync(id);
        if (entity != null)
        {
            _dbContext.Set<T>().Remove(entity);
        }
    }

    // Updated to match interface: List<T> (not IReadOnlyList<T>)
    public async Task<List<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    }

    // Updated to match interface: T? (nullable)
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    // NEW: Returns IQueryable for pagination and complex queries
    public IQueryable<T> GetQueryable()
    {
        return _dbContext.Set<T>().AsNoTracking();
    }
}
