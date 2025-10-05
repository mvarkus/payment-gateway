using System.Net;
using System.Text.Json;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.External.Mountebank;
using PaymentGateway.Api.Models.Internal;

namespace PaymentGateway.Api.Services;

public class MountebankAcquiringBankService : IAcquiringBankService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;

    public MountebankAcquiringBankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthorizePaymentResult> AuthorizePaymentAsync(
        string cardNumber,
        int expiryMonth,
        int expiryYear,
        string currency,
        int amount,
        string cvv)
    {
        try
        {
            using var httpResponseMessage = await _httpClient.PostAsJsonAsync(
                "/payments",
                new CreatePaymentRequest
                {
                    CardNumber = cardNumber,
                    ExpiryDate = expiryMonth.ToString().PadLeft(2, '0') + $"/{expiryYear}",
                    Currency = currency,
                    Amount = amount,
                    Cvv = cvv
                },
                _jsonSerializerOptions);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        var errorResponse = await httpResponseMessage.Content
                            .ReadFromJsonAsync<ErrorResponse>(_jsonSerializerOptions);

                        throw new AcquiringBankRejectedPayloadException(
                            errorResponse!.ErrorMessage,
                            AcquiringBankRejectedPayloadException.DefaultMessage);

                    case HttpStatusCode.ServiceUnavailable:
                        throw new AcquiringBankUnavailableException(
                            AcquiringBankUnavailableException.DefaultMessage);

                    default:
                        throw new AcquiringBankErrorException(
                            AcquiringBankErrorException.DefaultMessage);
                }
            }

            var response = await httpResponseMessage.Content
                .ReadFromJsonAsync<CreatePaymentResponse>(_jsonSerializerOptions);

            return new AuthorizePaymentResult
            {
                Authorized = response!.Authorized,
                AuthorizationCode = response.AuthorizationCode
            };
        }
        catch (TaskCanceledException e)
        {
            throw new AcquiringBankTimeoutException(AcquiringBankTimeoutException.DefaultMessage, e);
        }
    }
}
