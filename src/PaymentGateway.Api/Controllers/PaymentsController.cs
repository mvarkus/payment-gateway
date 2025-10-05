using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsService _paymentsService;

    public PaymentsController(IPaymentsService paymentsService)
    {
        _paymentsService = paymentsService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _paymentsService.GetPaymentAsync(id, cancellationToken);

        if (payment == null)
        {
            return new NotFoundObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Payment Not Found",
                Detail = $"Payment with ID '{id}' was not found."
            });
        }

        return new OkObjectResult(payment);
    }

    [HttpPost]
    public async Task<ActionResult<GetPaymentResponse>> PostPaymentAsync(PostPaymentRequest request)
    {
        var payment = await _paymentsService.CreatePaymentAsync(request);

        return new OkObjectResult(payment);
    }
}