namespace BuildingBlocks.Polly;

public class RetryOptions
{
    public int RetryCount { get; set; }
    public int SleepDuration { get; set; }
}
