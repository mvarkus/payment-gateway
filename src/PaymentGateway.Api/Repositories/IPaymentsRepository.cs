using PaymentGateway.Api.Models.Entities;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Payment payment);
}
