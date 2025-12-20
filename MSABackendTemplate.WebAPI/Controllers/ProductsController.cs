using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;

namespace MSABackendTemplate.WebAPI.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
