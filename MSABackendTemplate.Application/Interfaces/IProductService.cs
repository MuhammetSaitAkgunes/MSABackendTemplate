using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.Application.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync();
        Task<ServiceResult<ProductDto>> GetProductByIdAsync(Guid id);
        Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto request);
    }
}