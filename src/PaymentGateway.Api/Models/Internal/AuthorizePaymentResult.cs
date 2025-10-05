namespace PaymentGateway.Api.Models.Internal;

public class AuthorizePaymentResult
{
    public bool Authorized { get; set; }

    public required string AuthorizationCode { get; set; }
}
