namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankTimeoutException : Exception
{
    public const string DefaultMessage = "Request to acquiring bank timed out.";

    public AcquiringBankTimeoutException()
    {
    }

    public AcquiringBankTimeoutException(string? message) : base(message)
    {
    }

    public AcquiringBankTimeoutException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
