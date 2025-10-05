namespace PaymentGateway.Api.Options;

public class MountebankOption
{
    public static readonly string SectionName = "Mountebank";

    public required string BaseAddress { get; set; }

    public int TimeoutInSeconds { get; set; }
}
