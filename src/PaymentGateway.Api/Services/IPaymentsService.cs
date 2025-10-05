using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentsService
{
    Task<GetPaymentResponse?> GetPaymentAsync(Guid id, CancellationToken cancellationToken);

    Task<PostPaymentResponse> CreatePaymentAsync(PostPaymentRequest request);
}
