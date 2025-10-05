namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankErrorException : Exception
{
    public const string DefaultMessage = "An error occurred while processing the request with the acquiring bank.";

    public AcquiringBankErrorException()
    {
    }

    public AcquiringBankErrorException(string? message) : base(message)
    {
    }

    public AcquiringBankErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
