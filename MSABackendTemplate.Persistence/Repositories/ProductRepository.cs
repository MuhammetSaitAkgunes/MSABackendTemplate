using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Domain.Entities;
using MSABackendTemplate.Persistence.Contexts;

namespace MSABackendTemplate.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        // base(dbContext) diyerek GenericRepository'nin constructor'ını besliyoruz.
    }
}
