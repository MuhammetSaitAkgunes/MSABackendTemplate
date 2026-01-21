using AutoMapper;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Extensions;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Application.Parameters;
using MSABackendTemplate.Application.Wrappers;
using MSABackendTemplate.Domain.Common;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private const string CacheKeyPrefix = "products";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        public ProductService(IProductRepository productRepository, IMapper mapper, ICacheService cache)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto request)
        {
            // 1. MAPPING: DTO -> Entity
            var productEntity = _mapper.Map<Product>(request);

            // 2. LOGIC & PERSISTENCE
            await _productRepository.AddAsync(productEntity);

            // 3. CACHE INVALIDATION: Remove all product lists from cache
            await _cache.RemoveByPatternAsync($"{CacheKeyPrefix}:*");

            // 4. RETURN: Başarılı sonuç (Created - 201)
            return ServiceResult<Guid>.Success(productEntity.Id, 201);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync()
        {
            // CACHING: Try to get from cache first
            var cachedProducts = await _cache.GetAsync<List<ProductDto>>($"{CacheKeyPrefix}:all");
            if (cachedProducts != null)
                return ServiceResult<List<ProductDto>>.Success(cachedProducts);

            // Not in cache, fetch from database
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            // Store in cache
            await _cache.SetAsync($"{CacheKeyPrefix}:all", productDtos, CacheExpiration);

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        // NEW: Paginated products with sorting and filtering
        public async Task<ServiceResult<PagedResult<ProductDto>>> GetProductsPagedAsync(PaginationParameters parameters)
        {
            var query = _productRepository.GetQueryable();

            // FILTERING: Search by name or description
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchLower = parameters.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchLower) ||
                    p.Description.ToLower().Contains(searchLower));
            }

            // SORTING: Apply dynamic sorting
            query = query.ApplySorting(parameters.SortBy, parameters.SortOrder);

            // PAGINATION: Convert to paged result
            var pagedProducts = await query.ToPagedListAsync(parameters);

            // MAPPING: Entity -> DTO
            var pagedProductDtos = PagedResult<ProductDto>.Create(
                _mapper.Map<List<ProductDto>>(pagedProducts.Items),
                pagedProducts.TotalCount,
                pagedProducts.PageNumber,
                pagedProducts.PageSize);

            return ServiceResult<PagedResult<ProductDto>>.Success(pagedProductDtos);
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
