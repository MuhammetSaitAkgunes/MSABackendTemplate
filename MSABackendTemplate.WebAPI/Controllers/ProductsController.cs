using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Parameters;

namespace MSABackendTemplate.WebAPI.Controllers
{
    [Authorize] // JWT Token gerekli
    [EnableRateLimiting("fixed")] // 100 requests per minute
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // NEW: Paginated endpoint with query parameters
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PaginationParameters parameters)
        {
            return CreateActionResult(await _productService.GetProductsPagedAsync(parameters));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Tek satır! Servise git, sonucu al, BaseController'a ver.
            return CreateActionResult(await _productService.GetAllProductsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return CreateActionResult(await _productService.GetProductByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto request)
        {
            return CreateActionResult(await _productService.CreateProductAsync(request));
        }
    }
}
