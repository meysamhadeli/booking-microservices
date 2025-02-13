namespace BuildingBlocks.OpenTelemetryCollector;

public class ObservabilityOptions
{
    public string InstrumentationName { get; set; } = default!;
    public string? ServiceName { get; set; }
    public bool MetricsEnabled { get; set; } = true;
    public bool TracingEnabled { get; set; } = true;
    public bool LoggingEnabled { get; set; } = true;
    public bool UsePrometheusExporter { get; set; } = true;
    public bool UseOTLPExporter { get; set; } = true;
    public bool UseAspireOTLPExporter { get; set; } = true;
    public bool UseGrafanaExporter { get; set; }
    public bool UseConsoleExporter { get; set; }
    public bool UseJaegerExporter { get; set; }
    public bool UseZipkinExporter { get; set; }
    public ZipkinOptions ZipkinOptions { get; set; } = default!;
    public JaegerOptions JaegerOptions { get; set; } = default!;
    public OTLPOptions OTLPOptions { get; set; } = default!;
    public AspireDashboardOTLPOptions AspireDashboardOTLPOptions { get; set; } = default!;
}

// https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/src/OpenTelemetry.Exporter.Zipkin/README.md
public class ZipkinOptions
{
    /// <summary>
    /// Gets or sets endpoint address to receive telemetry
    /// </summary>
    public string HttpExporterEndpoint { get; set; } = "http://localhost:9411/api/v2/spans";
}

public class JaegerOptions
{
    public string OTLPGrpcExporterEndpoint { get; set; } = "http://localhost:14317";
    public string HttpExporterEndpoint { get; set; } = "http://localhost:14268/api/traces";
}

public class OTLPOptions
{
    public string OTLPGrpcExporterEndpoint { get; set; } = "http://localhost:4317";
    public string OTLPHttpExporterEndpoint { get; set; } = "http://localhost:4318";
}

public class AspireDashboardOTLPOptions
{
    public string OTLPGrpcExporterEndpoint { get; set; } = "http://localhost:4319";
}
