using Microsoft.EntityFrameworkCore;
using MSABackendTemplate.Domain.Common;
using MSABackendTemplate.Persistence.Contexts;
using static MSABackendTemplate.Application.Interfaces.Repositories.IGenericRepository;

namespace MSABackendTemplate.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    // DbContext'e ihtiyacımız var ama onu private tutuyoruz.
    private readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync(); // Transaction commit edilir.
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        // AsNoTracking: Sadece okuma yapacağımız zaman EF Core'un 
        // değişiklik takip mekanizmasını kapatır. Performansı %20-30 artırır.
        return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }
}
