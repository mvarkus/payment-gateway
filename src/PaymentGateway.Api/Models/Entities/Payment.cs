namespace PaymentGateway.Api.Models.Entities;

public class Payment
{
    public Guid Id { get; set; }

    public byte Status { get; set; }

    public required string LastFourCardDigits { get; set; }

    public int ExpiryMonth { get; set; }

    public int ExpiryYear { get; set; }

    public byte Currency { get; set; }

    public int Amount { get; set; }
}
