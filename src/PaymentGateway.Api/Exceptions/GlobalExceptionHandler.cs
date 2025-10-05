using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        string title = exception switch
        {
            AcquiringBankRejectedPayloadException => "Acquiring Bank Rejected Request",
            AcquiringBankUnavailableException => "Acquiring Bank Unavailable",
            _ => "Internal Server Error"
        };

        string detail = exception switch
        {
            AcquiringBankRejectedPayloadException => ((AcquiringBankRejectedPayloadException)exception).ResponseErrorMessage,
            AcquiringBankUnavailableException => exception.Message,
            _ => "An unhandled error occurred"
        };

        var responseBody = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = title,
            Detail = detail
        };

        await httpContext.Response.WriteAsJsonAsync(responseBody, cancellationToken);

        return true;
    }
}
