using AutoMapper;
using Microsoft.Extensions.Logging;
using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Wrappers;
using MSABackendTemplate.Domain.Entities;

namespace MSABackendTemplate.Application.Services;

/// <summary>
/// Order service demonstrating Unit of Work pattern with rollback.
/// Shows how to handle transactions across multiple repositories and external services.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IUnitOfWork unitOfWork,
        IPaymentService paymentService,
        IMapper mapper,
        ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Processes an order with automatic rollback on failure.
    /// 
    /// Transaction Flow:
    /// 1. Begin transaction
    /// 2. Check product exists and has stock
    /// 3. Decrease product stock
    /// 4. Create order
    /// 5. Save changes (not committed yet)
    /// 6. Process payment (external service)
    /// 7. If payment succeeds → Commit transaction
    /// 8. If payment fails → Rollback everything!
    /// </summary>
    public async Task<ServiceResult<OrderDto>> ProcessOrderAsync(CreateOrderDto request)
    {
        Order? order = null;

        try
        {
            _logger.LogInformation("Processing order for ProductId: {ProductId}, Quantity: {Quantity}",
                request.ProductId, request.Quantity);

            // Begin transaction
            await _unitOfWork.BeginTransactionAsync();

            // 1. Get product using generic repository
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
            if (product == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<OrderDto>.Failure("Product not found", 404);
            }

            // 2. Check stock availability
            if (product.Stock < request.Quantity)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult<OrderDto>.Failure(
                    $"Insufficient stock. Available: {product.Stock}, Requested: {request.Quantity}", 
                    400);
            }

            // 3. Decrease stock (this modifies the entity)
            product.DecreaseStock(request.Quantity);
            await _unitOfWork.Repository<Product>().UpdateAsync(product);

            // 4. Create order
            order = new Order(product.Id, request.Quantity, product.Price);
            await _unitOfWork.Repository<Order>().AddAsync(order);

            // 5. Save changes to database (but transaction not committed yet)
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Order created with ID: {OrderId}. Attempting payment...", order.Id);

            // 6. Process payment (external service - this is the critical point!)
            var paymentResult = await _paymentService.ProcessPaymentAsync(
                order.TotalAmount,
                request.PaymentToken);

            if (!paymentResult.IsSuccess)
            {
                // Payment failed! Rollback everything
                _logger.LogWarning("Payment failed for OrderId: {OrderId}. Rolling back...", order.Id);
                await _unitOfWork.RollbackTransactionAsync();

                return ServiceResult<OrderDto>.Failure(
                    $"Payment failed: {string.Join(", ", paymentResult.Errors ?? new List<string> { "Unknown error" })}",
                    paymentResult.StatusCode);
            }

            // 7. Payment succeeded! Mark order as paid and commit
            order.MarkAsPaid(paymentResult.Data!);
            await _unitOfWork.Repository<Order>().UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Commit transaction - everything is final now!
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order {OrderId} processed successfully", order.Id);

            var orderDto = _mapper.Map<OrderDto>(order);
            return ServiceResult<OrderDto>.Success(orderDto, 201);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order. Rolling back transaction.");
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<ServiceResult<OrderDto>> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);

        if (order == null)
            return ServiceResult<OrderDto>.Failure("Order not found", 404);

        var orderDto = _mapper.Map<OrderDto>(order);
        return ServiceResult<OrderDto>.Success(orderDto);
    }
}
