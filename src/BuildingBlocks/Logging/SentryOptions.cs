namespace BuildingBlocks.Logging;

public class SentryOptions
{
    public bool Enable { get; set; }
    public string Dsn { get; set; }
    public string MinimumBreadcrumbLevel { get; set; }
    public string MinimumEventLevel { get; set; }
}
