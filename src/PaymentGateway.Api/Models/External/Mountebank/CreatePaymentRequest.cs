namespace PaymentGateway.Api.Models.External.Mountebank;

public class CreatePaymentRequest
{
    public required string CardNumber { get; set; }

    public required string ExpiryDate { get; set; }

    public required string Currency { get; set; }

    public int Amount { get; set; }

    public required string Cvv { get; set; }
}
