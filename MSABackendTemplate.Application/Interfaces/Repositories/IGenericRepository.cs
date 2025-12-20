using MSABackendTemplate.Domain.Common;

namespace MSABackendTemplate.Application.Interfaces.Repositories;

public interface IGenericRepository
{
    // T, bir BaseEntity olmak zorundadır. (Constraint)
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
