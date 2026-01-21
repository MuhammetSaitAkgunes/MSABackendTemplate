using MSABackendTemplate.Domain.Common;

namespace MSABackendTemplate.Domain.Entities;

/// <summary>
/// Order entity representing a customer order.
/// Demonstrates transaction and rollback scenarios.
/// </summary>
public sealed class Order : BaseEntity
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? PaymentTransactionId { get; private set; }

    // EF Core requires a parameterless constructor
    private Order() { }

    public Order(Guid productId, int quantity, decimal unitPrice)
    {
        //Guard.AgainstNull(productId, nameof(productId));
        Guard.AgainstNegativeOrZero(quantity, nameof(quantity));
        Guard.AgainstNegativeOrZero(unitPrice, nameof(unitPrice));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalAmount = quantity * unitPrice;
        Status = OrderStatus.Pending;
    }

    public void MarkAsPaid(string paymentTransactionId)
    {
        Guard.AgainstNullOrEmpty(paymentTransactionId, nameof(paymentTransactionId));
        Guard.AgainstInvalidOperation(Status != OrderStatus.Pending, "Order is not in pending status");

        PaymentTransactionId = paymentTransactionId;
        Status = OrderStatus.Paid;
        MarkAsModified();
    }

    public void Cancel()
    {
        Guard.AgainstInvalidOperation(Status == OrderStatus.Paid, "Cannot cancel a paid order");

        Status = OrderStatus.Cancelled;
        MarkAsModified();
    }

    private void MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum OrderStatus
{
    Pending = 0,
    Paid = 1,
    Cancelled = 2
}
