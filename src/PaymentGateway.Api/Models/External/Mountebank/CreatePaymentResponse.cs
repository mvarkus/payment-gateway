namespace PaymentGateway.Api.Models.External.Mountebank;

public class CreatePaymentResponse
{
    public bool Authorized { get; set; }

    public required string AuthorizationCode { get; set; }
}
