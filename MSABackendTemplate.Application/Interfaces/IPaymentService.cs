using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.Application.Interfaces;

/// <summary>
/// Mock payment service interface.
/// In production, this would call a real payment gateway.
/// </summary>
public interface IPaymentService
{
    Task<ServiceResult<string>> ProcessPaymentAsync(decimal amount, string paymentToken);
    Task<ServiceResult> RefundAsync(string transactionId);
}
