using AutoMapper;
using FluentAssertions;
using Moq;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Interfaces.Repositories;
using MSABackendTemplate.Application.Services;
using MSABackendTemplate.Domain.Entities;
using System.Threading;
using Xunit;

namespace MSABackendTemplate.Tests.Application.Services;

/// <summary>
/// Unit tests for ProductService.
/// Tests business logic with mocked dependencies.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly ProductService _sut; // System Under Test

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<ICacheService>();
        _sut = new ProductService(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldReturnSuccess_WhenProductIsValid()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            Price = 1500,
            Stock = 10
        };

        var product = new Product("Laptop", "Gaming Laptop", 1500, 10);

        _mapperMock.Setup(m => m.Map<Product>(createDto))
            .Returns(product);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _cacheMock.Setup(c => c.RemoveByPatternAsync("products:*"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateProductAsync(createDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
        result.Data.Should().NotBeEmpty();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPatternAsync("products:*"), Times.Once);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnCachedData_WhenCacheHit()
    {
        // Arrange
        var cachedProducts = new List<ProductDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Description = "Desc 1", Price = 100 },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Description = "Desc 2", Price = 200 }
        };

        _cacheMock.Setup(c => c.GetAsync<List<ProductDto>>("products:all"))
            .ReturnsAsync(cachedProducts);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().BeEquivalentTo(cachedProducts);

        // Should NOT hit database
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<List<ProductDto>>(), It.IsAny<TimeSpan?>()), Times.Never);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldFetchFromDatabaseAndCache_WhenCacheMiss()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Product 1", "Desc 1", 100, 10),
            new("Product 2", "Desc 2", 200, 20)
        };

        var productDtos = new List<ProductDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Description = "Desc 1", Price = 100 },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Description = "Desc 2", Price = 200 }
        };

        _cacheMock.Setup(c => c.GetAsync<List<ProductDto>>("products:all"))
            .ReturnsAsync((List<ProductDto>?)null); // Cache MISS

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);

        _mapperMock.Setup(m => m.Map<List<ProductDto>>(products))
            .Returns(productDtos);

        _cacheMock.Setup(c => c.SetAsync<List<ProductDto>>(
                "products:all",
                It.IsAny<List<ProductDto>>(),
                It.IsAny<TimeSpan?>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _cacheMock.Verify(c => c.SetAsync<List<ProductDto>>("products:all", It.IsAny<List<ProductDto>>(), It.IsAny<TimeSpan?>()), Times.Once);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product("Laptop", "Gaming", 1500, 10);
        var productDto = new ProductDto { Id = productId, Name = "Laptop", Description = "Gaming", Price = 1500 };

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mapperMock.Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Laptop");
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.GetProductByIdAsync(productId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }
}
