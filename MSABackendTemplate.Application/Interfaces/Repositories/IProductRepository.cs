using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Interfaces.Repositories;


// IGenericRepository'den miras alarak onun tüm özelliklerine sahip olur.
// Ekstra metot gerekirse buraya yazarız.
public interface IProductRepository: IGenericRepository.IGenericRepository<Product>
{
    // Örnek: Task<IReadOnlyList<Product>> GetProductsByMinPrice(decimal minPrice);
    // Şimdilik boş bırakıyoruz, Generic yetiyor.
}
