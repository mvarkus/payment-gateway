using PaymentGateway.Api.Models.Internal;

namespace PaymentGateway.Api.Services;

public interface IAcquiringBankService
{
    Task<AuthorizePaymentResult> AuthorizePaymentAsync(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv);
}
