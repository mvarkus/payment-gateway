namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankRejectedPayloadException : Exception
{
    public string ResponseErrorMessage { get; init; }
    public const string DefaultMessage = "Acquiring bank rejected the payment request payload.";

    public AcquiringBankRejectedPayloadException(string responseErrorMessage)
    {
        ResponseErrorMessage = responseErrorMessage;
    }

    public AcquiringBankRejectedPayloadException(string responseErrorMessage, string? message) : base(message)
    {
        ResponseErrorMessage = responseErrorMessage;
    }

    public AcquiringBankRejectedPayloadException(string responseErrorMessage, string? message, Exception? innerException)
        : base(message, innerException)
    {
        ResponseErrorMessage = responseErrorMessage;
    }
}
