using MSABackendTemplate.Application.Interfaces;
using MSABackendTemplate.Application.Wrappers;

namespace MSABackendTemplate.Persistence.Services;

/// <summary>
/// Mock payment service for demonstration.
/// In production, replace with real payment gateway integration.
/// </summary>
public class MockPaymentService : IPaymentService
{
    public async Task<ServiceResult<string>> ProcessPaymentAsync(decimal amount, string paymentToken)
    {
        // Simulate API call delay
        await Task.Delay(100);

        // Simulate payment failure for demonstration
        // In real scenario, this would call Stripe, PayPal, etc.
        if (paymentToken == "FAIL" || paymentToken == "INVALID")
        {
            return ServiceResult<string>.Failure("Payment declined by bank", 402);
        }

        // Generate mock transaction ID
        var transactionId = $"TXN_{Guid.NewGuid():N}";
        return ServiceResult<string>.Success(transactionId);
    }

    public async Task<ServiceResult> RefundAsync(string transactionId)
    {
        // Simulate refund process
        await Task.Delay(100);
        return ServiceResult.Success();
    }
}
