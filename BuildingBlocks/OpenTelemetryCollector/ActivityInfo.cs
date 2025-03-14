using System.Diagnostics;

namespace BuildingBlocks.OpenTelemetryCollector;

public class ActivityInfo
{
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public TimeSpan Duration { get; set; }
    public string Status { get; set; } = default!;
    public string? StatusDescription { get; set; }
    public IDictionary<string, string?> Tags { get; set; } = new Dictionary<string, string?>();
    public IList<ActivityEventInfo> Events { get; set; } = new List<ActivityEventInfo>();
    public string TraceId { get; set; } = default!;
    public string SpanId { get; set; } = default!;

    public string? ParentId { get; set; }

    public ActivityContext? Parent { get; set; }

    public ActivityKind Kind { get; set; }
}

public class ActivityEventInfo
{
    public string Name { get; set; } = default!;
    public DateTimeOffset Timestamp { get; set; }
    public IDictionary<string, object?> Attributes { get; set; } = new Dictionary<string, object?>();
}
