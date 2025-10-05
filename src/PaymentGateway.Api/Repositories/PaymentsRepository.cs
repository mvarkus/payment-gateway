using PaymentGateway.Api.Models.Entities;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid, Payment> _payments = new();

    public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _payments.TryGetValue(id, out var payment);

        return payment;
    }

    public async Task AddAsync(Payment payment)
    {
        _payments[payment.Id] = payment;
    }
}