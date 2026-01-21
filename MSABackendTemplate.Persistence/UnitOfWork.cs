using Microsoft.EntityFrameworkCore.Storage;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Domain.Common;
using MSABackendTemplate.Persistence.Contexts;
using MSABackendTemplate.Persistence.Repositories;

namespace MSABackendTemplate.Persistence;

/// <summary>
/// Generic Unit of Work implementation.
/// Manages transactions across multiple repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    /// <summary>
    /// Gets a repository for the specified entity type.
    /// Creates repository on first access and caches it.
    /// </summary>
    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);

        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<T>)_repositories[type];
        }

        var repository = new GenericRepository<T>(_context);
        _repositories.Add(type, repository);
        return repository;
    }

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits the current transaction.
    /// All changes will be persisted to the database.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("Transaction not started. Call BeginTransactionAsync first.");

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// All changes will be discarded.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null)
            return; // No active transaction

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    /// <summary>
    /// Saves changes without committing transaction.
    /// Useful for intermediate saves.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the Unit of Work and any active transaction.
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
