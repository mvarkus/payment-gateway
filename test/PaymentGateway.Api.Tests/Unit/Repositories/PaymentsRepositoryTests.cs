using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Tests.Unit.Repositories;

public class PaymentsRepositoryTests
{
    [Fact]
    public async Task PaymentsCanBeAddedAndRetrievedById()
    {
        // Arrange
        var random = new Random();

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = 1,
            LastFourCardDigits = random.Next(1111, 9999).ToString(),
            ExpiryYear = random.Next(2023, 2030),
            ExpiryMonth = random.Next(1, 12),
            Currency = 3,
            Amount = random.Next(1, 10000)
        };

        var paymentsRepository = new PaymentsRepository();

        // Act
        await paymentsRepository.AddAsync(payment);
        var retrievedPayment = await paymentsRepository.GetByIdAsync(payment.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(retrievedPayment);
        Assert.Equal(payment.Id, retrievedPayment!.Id);
        Assert.Equal(payment.Status, retrievedPayment.Status);
        Assert.Equal(payment.LastFourCardDigits, retrievedPayment.LastFourCardDigits);
        Assert.Equal(payment.ExpiryMonth, retrievedPayment.ExpiryMonth);
        Assert.Equal(payment.ExpiryYear, retrievedPayment.ExpiryYear);
        Assert.Equal(payment.Currency, retrievedPayment.Currency);
        Assert.Equal(payment.Amount, retrievedPayment.Amount);
    }
}
