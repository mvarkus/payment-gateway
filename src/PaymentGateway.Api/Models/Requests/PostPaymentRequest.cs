using System.ComponentModel.DataAnnotations;

using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Requests;

public class PostPaymentRequest : IValidatableObject
{
    [Required]
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be between 14 and 19 digits.")]
    public string? CardNumber { get; set; }

    [Required]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12.")]
    public int? ExpiryMonth { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Expiry year must be a positive")]
    public int? ExpiryYear { get; set; }

    [Required]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be 3 uppercase characters")]
    public string? Currency { get; set; }

    [Required]
    public int? Amount { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    public string? Cvv { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsValidExpiryDate(ExpiryMonth!.Value, ExpiryYear!.Value))
        {
            yield return new ValidationResult("Card expiry date must be in the future.", ["expiry_month", "expiry_year"]);
        }

        if (!Enum.TryParse<Currency>(Currency, false, out _))
        {
            yield return new ValidationResult("Currency must be a valid currency code.", ["currency"]);
        }
    }

    private static bool IsValidExpiryDate(int month, int year)
    {
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;

        return (currentYear < year) || (currentYear == year && currentMonth < month);
    }
}