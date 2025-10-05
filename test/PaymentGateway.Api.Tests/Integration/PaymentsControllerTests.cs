using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Options;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Tests.Helpers;

namespace PaymentGateway.Api.Tests.Integration;

public class PaymentsControllerTests
{
    private readonly Random _random = new();

    [Fact]
    public async Task Get_RetrievesAPaymentSuccessfully()
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

        var paymentsRepository = new PaymentsRepository();
        await paymentsRepository.AddAsync(payment);

        var client = GetClient(paymentsRepository);

        // Act
        var response = await client.GetAsync($"/api/payments/{payment.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>(JsonSerializerOptionsHelper.SnakeCaseLower);
        Assert.NotNull(paymentResponse);
        Assert.Equal(payment.Id, paymentResponse!.Id);
        Assert.Equal(((PaymentStatus)payment.Status).ToString(), paymentResponse.Status);
        Assert.Equal(payment.LastFourCardDigits, paymentResponse.LastFourCardDigits);
        Assert.Equal(payment.ExpiryMonth, paymentResponse.ExpiryMonth);
        Assert.Equal(payment.ExpiryYear, paymentResponse.ExpiryYear);
        Assert.Equal(((Currency)payment.Currency).ToString(), paymentResponse.Currency);
        Assert.Equal(payment.Amount, paymentResponse.Amount);
    }

    [Fact]
    public async Task Get_Returns404IfPaymentNotFound()
    {
        // Arrange
        var client = GetClient();

        // Act
        var response = await client.GetAsync($"/api/payments/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreatedPayment()
    {
        // Arrange
        var paymentsStorage = new List<Payment>();
        var request = new PostPaymentRequest
        {
            CardNumber = "2222405343248877",
            ExpiryMonth = 6,
            ExpiryYear = DateTime.UtcNow.Year + 1,
            Currency = Currency.GBP.ToString(),
            Amount = 1000,
            Cvv = "123"
        };

        var fakePaymentsRepository = new Mock<IPaymentsRepository>();
        fakePaymentsRepository.Setup(repo => repo.AddAsync(It.IsAny<Payment>()))
            .Callback((Payment payment) => paymentsStorage.Add(payment));

        var client = GetClient(fakePaymentsRepository.Object);

        // Act
        var response = await client.PostAsJsonAsync(
            $"/api/payments",
            request,
            JsonSerializerOptionsHelper.SnakeCaseLower);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(
            JsonSerializerOptionsHelper.SnakeCaseLower);
        Assert.NotNull(paymentResponse);

        Assert.Single(paymentsStorage);
        var payment = paymentsStorage[0];

        Assert.Equal(payment.Id, paymentResponse!.Id);
        Assert.Equal(PaymentStatus.Authorized.ToString(), paymentResponse.Status);
        Assert.Equal(payment.LastFourCardDigits, paymentResponse.LastFourCardDigits);
        Assert.Equal(payment.ExpiryMonth, paymentResponse.ExpiryMonth);
        Assert.Equal(payment.ExpiryYear, paymentResponse.ExpiryYear);
        Assert.Equal(((Currency)payment.Currency).ToString(), paymentResponse.Currency);
        Assert.Equal(payment.Amount, paymentResponse.Amount);
    }

    private static HttpClient GetClient(IPaymentsRepository? paymentsRepository = null)
    {
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();

        return webApplicationFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                if (paymentsRepository is not null)
                {
                    services.AddSingleton(s => paymentsRepository);
                }

                services.Configure<MountebankOption>(opts =>
                {
                    opts.BaseAddress = Environment.GetEnvironmentVariable("TEST_ACQUIRING_BANK_BASE_ADDRESS")
                        ?? "http://localhost:8080";
                    opts.TimeoutInSeconds = 30;
                });
            });
        }).CreateClient();
    }
}