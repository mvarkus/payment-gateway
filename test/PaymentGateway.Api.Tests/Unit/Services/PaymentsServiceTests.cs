using Moq;

using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Internal;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.Unit.Services;

public class PaymentsServiceTests
{
    private readonly Random _random = new();

    [Fact]
    public async Task GetPaymentAsync_ReturnsNullIfPaymentNotFound()
    {
        // Arrange
        var fakePaymentsRepository = new Mock<IPaymentsRepository>();

        fakePaymentsRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Payment?)null);

        var paymentsService = GetService(paymentsRepository: fakePaymentsRepository.Object);

        // Act
        var result = await paymentsService.GetPaymentAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPaymentAsync_ReturnsPaymentIfPaymentFound()
    {
        // Arrange
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Status = (byte)PaymentStatus.Authorized,
            LastFourCardDigits = _random.Next(1111, 9999).ToString(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Currency = (byte)Currency.GBP,
            Amount = _random.Next(1, 10000)
        };

        var fakePaymentsRepository = new Mock<IPaymentsRepository>();
        fakePaymentsRepository.Setup(repo => repo.GetByIdAsync(payment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var paymentsService = GetService(paymentsRepository: fakePaymentsRepository.Object);

        // Act
        var result = await paymentsService.GetPaymentAsync(payment.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payment.Id, result!.Id);
        Assert.Equal(((PaymentStatus)payment.Status).ToString(), result.Status);
        Assert.Equal(payment.LastFourCardDigits, result.LastFourCardDigits);
        Assert.Equal(payment.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(payment.ExpiryYear, result.ExpiryYear);
        Assert.Equal(((Currency)payment.Currency).ToString(), result.Currency);
        Assert.Equal(payment.Amount, result.Amount);
    }

    [Theory]
    [InlineData(true, PaymentStatus.Authorized)]
    [InlineData(false, PaymentStatus.Declined)]
    public async Task CreatePaymentAsync_CreatesPaymentIfAcquiringBankAcceptedRequest(
        bool authorized,
        PaymentStatus paymentStatus)
    {
        // Arrange
        var cardNumber = _random.Next(1111, 9999).ToString() +
                         _random.Next(1111, 9999).ToString() +
                         _random.Next(1111, 9999).ToString() +
                         _random.Next(1111, 9999).ToString();

        var request = new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = _random.Next(1, 12),
            ExpiryYear = _random.Next(2023, 2030),
            Currency = Currency.GBP.ToString(),
            Amount = _random.Next(1, 10000),
            Cvv = _random.Next(100, 999).ToString()
        };

        var fakeAcquiringBank = new Mock<IAcquiringBankService>();
        fakeAcquiringBank.Setup(bank => bank.AuthorizePaymentAsync(
                request.CardNumber,
                (int)request.ExpiryMonth,
                (int)request.ExpiryYear,
                request.Currency.ToString(),
                (int)request.Amount,
                request.Cvv))
            .ReturnsAsync(new AuthorizePaymentResult { Authorized = authorized, AuthorizationCode = "code" });

        var paymentsService = GetService(acquiringBankService: fakeAcquiringBank.Object);

        // Act
        var result = await paymentsService.CreatePaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(paymentStatus.ToString(), result.Status);
        Assert.Equal(cardNumber[^4..], result.LastFourCardDigits);
        Assert.Equal(request.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, result.ExpiryYear);
        Assert.Equal(request.Currency, result.Currency);
        Assert.Equal(request.Amount, result.Amount);
    }

    private static PaymentsService GetService(
        IAcquiringBankService? acquiringBankService = null,
        IPaymentsRepository? paymentsRepository = null)
    {
        return new PaymentsService(
            acquiringBankService ?? new Mock<IAcquiringBankService>().Object,
            paymentsRepository ?? new Mock<IPaymentsRepository>().Object);
    }
}
