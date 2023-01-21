namespace BuildingBlocks.Polly;

public class CircuitBreakerOptions
{
    public int RetryCount { get; set; }
    public int BreakDuration { get; set; }
}
