using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Services;

public class PaymentsService : IPaymentsService
{
    private readonly IAcquiringBankService _acquiringBankService;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsService(IAcquiringBankService acquiringBankService, IPaymentsRepository paymentsRepository)
    {
        _acquiringBankService = acquiringBankService;
        _paymentsRepository = paymentsRepository;
    }

    public async Task<GetPaymentResponse?> GetPaymentAsync(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _paymentsRepository.GetByIdAsync(id, cancellationToken);

        if (payment == null)
        {
            return null;
        }

        return new GetPaymentResponse
        {
            Id = payment.Id,
            Status = ((PaymentStatus)payment.Status).ToString(),
            LastFourCardDigits = payment.LastFourCardDigits,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = ((Currency)payment.Currency).ToString(),
            Amount = payment.Amount
        };
    }

    public async Task<PostPaymentResponse> CreatePaymentAsync(PostPaymentRequest request)
    {
        var authorizePaymentResult = await _acquiringBankService.AuthorizePaymentAsync(
            request.CardNumber!,
            request.ExpiryMonth!.Value,
            request.ExpiryYear!.Value,
            request.Currency!,
            request.Amount!.Value,
            request.Cvv!);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = (byte)(authorizePaymentResult.Authorized
                    ? PaymentStatus.Authorized
                    : PaymentStatus.Declined),
            LastFourCardDigits = request.CardNumber![^4..],
            ExpiryMonth = request.ExpiryMonth.Value,
            ExpiryYear = request.ExpiryYear.Value,
            Currency = (byte)Enum.Parse<Currency>(request.Currency!),
            Amount = request.Amount.Value
        };

        await _paymentsRepository.AddAsync(payment);

        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = ((PaymentStatus)payment.Status).ToString(),
            LastFourCardDigits = payment.LastFourCardDigits,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = ((Currency)payment.Currency).ToString(),
            Amount = payment.Amount
        };
    }
}
