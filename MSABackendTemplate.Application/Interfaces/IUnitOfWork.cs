using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Domain.Common;

namespace MSABackendTemplate.Application.Interfaces;

/// <summary>
/// Generic Unit of Work pattern for managing transactions across multiple repositories.
/// Provides automatic rollback on failure.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets a generic repository for any entity type.
    /// Example: unitOfWork.Repository&lt;Product&gt;()
    /// </summary>
    IGenericRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Begins a new database transaction.
    /// All operations after this will be part of the transaction.
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits all changes in the current transaction.
    /// Use this when all operations succeed.
    /// </summary>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back all changes in the current transaction.
    /// Use this when any operation fails.
    /// </summary>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Saves changes to the database without committing transaction.
    /// Useful for intermediate saves within a transaction.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
