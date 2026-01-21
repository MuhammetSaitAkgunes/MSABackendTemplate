namespace MSABackendTemplate.Application.DTOs;

public class CreateOrderDto
{
    public required Guid ProductId { get; set; }
    public required int Quantity { get; set; }
    public required string PaymentToken { get; set; } // Simulated payment token
}

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentTransactionId { get; set; }
}
