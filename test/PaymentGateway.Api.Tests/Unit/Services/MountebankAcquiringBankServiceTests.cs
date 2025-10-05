using System.Net;
using System.Text.Json;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.External.Mountebank;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Tests.Helpers;

namespace PaymentGateway.Api.Tests.Unit.Services;

public class MountebankAcquiringBankServiceTests
{
    private readonly Mock<HttpMessageHandler> _fakeMessageHandler = new();

    [Fact]
    public async Task AuthorizePaymentAsync_ThrowsAcquiringBankTimeoutExceptionIfRequestHitTimeout()
    {
        // Arrange
        _fakeMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Throws(new TaskCanceledException());

        var mountebankAcquiringBank = GetService();

        // Act
        var act = async () => await mountebankAcquiringBank.AuthorizePaymentAsync("1234123412341234", 12, 2025, "GBP", 1000, "123");

        // Assert
        await Assert.ThrowsAsync<AcquiringBankTimeoutException>(act);
    }

    [Fact]
    public async Task AuthorizePaymentAsync_ThrowsAcquiringBankRejectedPayloadExceptionIfResponseIsBadRequest()
    {
        // Arrange
        var errorJson = JsonSerializer.Serialize(
            new ErrorResponse { ErrorMessage = "Invalid card number" },
            JsonSerializerOptionsHelper.SnakeCaseLower);

        _fakeMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(errorJson),
                StatusCode = HttpStatusCode.BadRequest
            });

        var mountebankAcquiringBank = GetService();

        // Act
        var act = async () => await mountebankAcquiringBank.AuthorizePaymentAsync("1234123412341234", 12, 2025, "GBP", 1000, "123");

        // Assert
        await Assert.ThrowsAsync<AcquiringBankRejectedPayloadException>(act);
    }

    [Fact]
    public async Task AuthorizePaymentAsync_ThrowsAcquiringBankUnavailableExceptionIfResponseIsServiceUnavailable()
    {
        // Arrange
        _fakeMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.ServiceUnavailable
            });

        var mountebankAcquiringBank = GetService();

        // Act
        var act = async () => await mountebankAcquiringBank.AuthorizePaymentAsync("1234123412341234", 12, 2025, "GBP", 1000, "123");

        // Assert
        await Assert.ThrowsAsync<AcquiringBankUnavailableException>(act);
    }

    [Fact]
    public async Task AuthorizePaymentAsync_ThrowsAcquiringBankErrorExceptionIfRequestIsNotSuccessful()
    {
        // Arrange
        _fakeMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var mountebankAcquiringBank = GetService();

        // Act
        var act = async () => await mountebankAcquiringBank.AuthorizePaymentAsync("1234123412341234", 12, 2025, "GBP", 1000, "123");

        // Assert
        await Assert.ThrowsAsync<AcquiringBankErrorException>(act);
    }

    [Fact]
    public async Task AuthorizePaymentAsync_RetrunsAuthorizationResultIfResponseIsSuccessful()
    {
        // Arrange
        var response = new CreatePaymentResponse { Authorized = true, AuthorizationCode = "code" };
        var responseJson = JsonSerializer.Serialize(response, JsonSerializerOptionsHelper.SnakeCaseLower);

        _fakeMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(responseJson),
                StatusCode = HttpStatusCode.OK
            });

        var mountebankAcquiringBank = GetService();

        // Act
        var result = await mountebankAcquiringBank.AuthorizePaymentAsync("1234123412341234", 12, 2025, "GBP", 1000, "123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response.Authorized, result.Authorized);
        Assert.Equal(response.AuthorizationCode, result.AuthorizationCode);
    }

    private MountebankAcquiringBankService GetService()
    {
        var httpClient = new HttpClient(_fakeMessageHandler.Object)
        {
            BaseAddress = new Uri("http://mocked-test.test")
        };

        return new MountebankAcquiringBankService(httpClient);
    }
}
