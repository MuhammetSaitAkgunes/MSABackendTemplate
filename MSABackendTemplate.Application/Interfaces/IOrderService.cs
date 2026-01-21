using MSABackendTemplate.Application.DTOs;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.Application.Interfaces;

/// <summary>
/// Order service interface.
/// Demonstrates transaction and rollback scenarios.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Processes an order with automatic rollback on payment failure.
    /// Example:
    /// - Stock decreases
    /// - Order created
    /// - Payment processed
    /// - If payment fails → Everything rolls back!
    /// </summary>
    Task<ServiceResult<OrderDto>> ProcessOrderAsync(CreateOrderDto request);

    Task<ServiceResult<OrderDto>> GetOrderByIdAsync(Guid orderId);
}
