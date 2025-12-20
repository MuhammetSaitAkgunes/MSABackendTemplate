using AutoMapper;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Application.Wrappers;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto request)
        {
            // 1. MAPPING: DTO -> Entity
            var productEntity = _mapper.Map<Product>(request);

            // 2. LOGIC & PERSISTENCE
            await _productRepository.AddAsync(productEntity);

            // 3. RETURN: Başarılı sonuç (Created - 201)
            return ServiceResult<Guid>.Success(productEntity.Id, 201);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync()
        {
            throw new Exception("Veritabanı kablosunu kedi kemirdi!");

            var products = await _productRepository.GetAllAsync();

            // Veri yoksa boş liste dön, hata değil.
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            // GUARD CLAUSE: Ürün yoksa 404 dön
            if (product == null)
            {
                return ServiceResult<ProductDto>.Failure("Product not found.", 404);
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return ServiceResult<ProductDto>.Success(productDto);
        }
    }
}
