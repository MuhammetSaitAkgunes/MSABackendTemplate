using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Interfaces.Repositories;

/// <summary>
/// Product-specific repository interface.
/// Inherits from IGenericRepository for common CRUD operations.
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    // Product-specific methods can be added here in the future
    // For now, generic CRUD operations are sufficient
}
