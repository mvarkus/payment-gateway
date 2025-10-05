namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankUnavailableException : Exception
{
    public const string DefaultMessage = "Acquiring bank service is unavailable.";

    public AcquiringBankUnavailableException()
    {
    }

    public AcquiringBankUnavailableException(string? message) : base(message)
    {
    }

    public AcquiringBankUnavailableException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
