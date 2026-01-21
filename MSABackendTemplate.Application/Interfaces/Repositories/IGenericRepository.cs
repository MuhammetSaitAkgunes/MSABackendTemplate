using MSABackendTemplate.Domain.Common;

namespace MSABackendTemplate.Application.Interfaces.Repositories;

/// <summary>
/// Generic repository interface for common CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets all entities from the database.
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity by its ID.
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Returns an IQueryable for complex queries and pagination.
    /// </summary>
    IQueryable<T> GetQueryable();
}
