using System.Text.Json;

namespace PaymentGateway.Api.Tests.Helpers;

internal static class JsonSerializerOptionsHelper
{
    public static readonly JsonSerializerOptions SnakeCaseLower = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
}
