using System.Diagnostics;
using System.Reflection;
using BuildingBlocks.OpenTelemetryCollector.CoreDiagnostics.Commands;
using BuildingBlocks.OpenTelemetryCollector.CoreDiagnostics.Query;
using BuildingBlocks.OpenTelemetryCollector.DiagnosticsProvider;
using BuildingBlocks.Web;
using Grafana.OpenTelemetry;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetryCollector;

// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-otlp-example
// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-prgrja-example
// https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-prgrja-example
// https://blog.codingmilitia.com/2023/09/05/observing-dotnet-microservices-with-opentelemetry-logs-traces-metrics/
public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static WebApplicationBuilder AddCustomObservability(this WebApplicationBuilder builder)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        builder.Services.AddSingleton<IDiagnosticsProvider, CustomeDiagnosticsProvider>();
        builder.AddCoreDiagnostics();

        builder.Services.AddValidateOptions<ObservabilityOptions>();
        var observabilityOptions = builder.Services.GetOptions<ObservabilityOptions>(nameof(ObservabilityOptions));

        // InstrumentationName property option is mandatory and can't be empty
        ArgumentException.ThrowIfNullOrEmpty(observabilityOptions.InstrumentationName);
        ObservabilityConstant.InstrumentationName = observabilityOptions.InstrumentationName;

        if (observabilityOptions is { MetricsEnabled: false, TracingEnabled: false, LoggingEnabled: false })
        {
            return builder;
        }

        void ConfigureResourceBuilder(ResourceBuilder resourceBuilder)
        {
            resourceBuilder.AddAttributes([new("service.environment", builder.Environment.EnvironmentName)]);

            resourceBuilder.AddService(
                serviceName: observabilityOptions.ServiceName ?? builder.Environment.ApplicationName,
                serviceVersion: Assembly.GetCallingAssembly().GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName
            );
        }

        if (observabilityOptions.LoggingEnabled)
        {
            // logging
            // opentelemtry logging works with .net default logging providers and doesn't work for `serilog`, in serilog we should enable `WriteToProviders=true`
            builder.Logging.AddOpenTelemetry(options =>
            {
                var resourceBuilder = ResourceBuilder.CreateDefault();
                ConfigureResourceBuilder(resourceBuilder);
                options.SetResourceBuilder(resourceBuilder);

                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                // this allows the state value passed to the logger.Log method to be parsed, in case it isn't a collection of KeyValuePair<string, object?>, which is the case when we use things like logger.LogInformation.
                options.ParseStateValues = true;
                // which means the message wouldn't have the placeholders replaced
                options.IncludeFormattedMessage = true;

                // add some metadata to exported logs
                options.SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddService(
                            observabilityOptions.ServiceName ?? builder.Environment.ApplicationName,
                            serviceVersion: Assembly.GetCallingAssembly().GetName().Version?.ToString() ?? "unknown",
                            serviceInstanceId: Environment.MachineName
                        )
                );

                options.AddLoggingExporters(observabilityOptions);
            });
        }

        if (observabilityOptions is { MetricsEnabled: false, TracingEnabled: false })
        {
            return builder;
        }

        OpenTelemetryBuilder otel = null!;

        if (observabilityOptions.MetricsEnabled || observabilityOptions.TracingEnabled)
        {
            // metrics and tracing
            otel = builder.Services.AddOpenTelemetry();
            otel.ConfigureResource(ConfigureResourceBuilder);
        }

        if (observabilityOptions.MetricsEnabled)
        {
            otel.WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddMeter(observabilityOptions.InstrumentationName)
                    // metrics provides by ASP.NET Core in .NET 8
                    .AddView(
                        "http.server.request.duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10],
                        }
                    )
                    .AddMeter("System.Runtime")
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                AddMetricsExporter(observabilityOptions, metrics);
            });
        }

        if (observabilityOptions.TracingEnabled)
        {
            otel.WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler(new AlwaysOnSampler());
                }

                tracing
                    .SetErrorStatusOnException()
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        // Don't trace requests to the health endpoint to avoid filling the dashboard with noise
                        options.Filter = httpContext =>
                                             !(httpContext.Request.Path.StartsWithSegments(
                                                   HealthEndpointPath, StringComparison.OrdinalIgnoreCase) ||
                                               httpContext.Request.Path.StartsWithSegments(
                                                   AlivenessEndpointPath, StringComparison.OrdinalIgnoreCase
                                               ));
                    })
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation(instrumentationOptions =>
                    {
                        instrumentationOptions.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(instrumentationOptions =>
                    {
                        instrumentationOptions.SetDbStatementForText = true;
                    })
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddNpgsql()
                    // `AddSource` for adding custom activity sources
                    .AddSource(observabilityOptions.InstrumentationName)
                    // metrics provides by ASP.NET Core in .NET 8
                    .AddSource("Microsoft.AspNetCore.Hosting")
                    .AddSource("Microsoft.AspNetCore.Server.Kestrel");

                AddTracingExporter(observabilityOptions, tracing);
            });
        }

        return builder;
    }


    public static WebApplication UseCustomObservability(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<ObservabilityOptions>>().Value;

        app.Use(
            async (context, next) =>
            {
                var metricsFeature = context.Features.Get<IHttpMetricsTagsFeature>();
                if (metricsFeature != null && context.Request.Path is { Value: "/metrics" or "/health" })
                {
                    metricsFeature.MetricsDisabled = true;
                }

                await next(context);
            }
        );

        if (options.UsePrometheusExporter)
        {
            // export application metrics in `/metrics` endpoint and should scrape in the Prometheus config file and `scrape_configs`
            // https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/src/OpenTelemetry.Exporter.Prometheus.AspNetCore
            app.UseOpenTelemetryPrometheusScrapingEndpoint(); // http://localhost:4000/metrics
        }

        return app;
    }

    private static void AddTracingExporter(ObservabilityOptions observabilityOptions, TracerProviderBuilder tracing)
    {
        if (observabilityOptions.UseJaegerExporter)
        {
            ArgumentNullException.ThrowIfNull(observabilityOptions.JaegerOptions);
            // https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/docs/trace/getting-started-jaeger
            // `OpenTelemetry.Exporter.Jaeger` package and `AddJaegerExporter` to use Http endpoint (http://localhost:14268/api/traces) is deprecated, and we should use `OpenTelemetry.Exporter.OpenTelemetryProtocol` and `AddOtlpExporter` with OTLP port `4317` on Jaeger
            // tracing.AddJaegerExporter(
            //     x => x.Endpoint = new Uri(observabilityOptions.JaegerOptions.HttpExporterEndpoint)); // http://localhost:14268/api/traces
            tracing.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.JaegerOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseZipkinExporter)
        {
            ArgumentNullException.ThrowIfNull(observabilityOptions.ZipkinOptions);
            // https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/src/OpenTelemetry.Exporter.Zipkin
            tracing.AddZipkinExporter(x =>
                x.Endpoint = new Uri(observabilityOptions.ZipkinOptions.HttpExporterEndpoint)
            ); // "http://localhost:9411/api/v2/spans"
        }

        if (observabilityOptions.UseConsoleExporter)
        {
            tracing.AddConsoleExporter();
        }

        if (observabilityOptions.UseOTLPExporter)
        {
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            tracing.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.OTLPOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseAspireOTLPExporter)
        {
            // we can just one `AddOtlpExporter` and in development use `aspire-dashboard` OTLP endpoint address as `OTLPExporterEndpoint` and in production we can use `otel-collector` OTLP endpoint address
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            tracing.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.AspireDashboardOTLPOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseGrafanaExporter)
        {
            // https://github.com/grafana/grafana-opentelemetry-dotnet/blob/main/docs/configuration.md#aspnet-core
            // https://github.com/grafana/grafana-opentelemetry-dotnet/
            // https://github.com/grafana/grafana-opentelemetry-dotnet/blob/main/docs/configuration.md#sending-to-an-agent-or-collector-via-otlp
            // https://grafana.com/docs/grafana-cloud/monitor-applications/application-observability/instrument/dotnet/
            tracing.UseGrafana();
        }
    }

    private static void AddMetricsExporter(ObservabilityOptions observabilityOptions, MeterProviderBuilder metrics)
    {
        if (observabilityOptions.UsePrometheusExporter)
        {
            // https://github.com/open-telemetry/opentelemetry-dotnet/tree/e330e57b04fa3e51fe5d63b52bfff891fb5b7961/src/OpenTelemetry.Exporter.Prometheus.AspNetCore
            // for exporting app metrics to `/metrics` endpoint
            metrics.AddPrometheusExporter(o => o.DisableTotalNameSuffixForCounters = true); // http://localhost:4000/metrics
        }

        if (observabilityOptions.UseConsoleExporter)
        {
            metrics.AddConsoleExporter();
        }

        if (observabilityOptions.UseOTLPExporter)
        {
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            metrics.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.OTLPOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseAspireOTLPExporter)
        {
            // we can just one `AddOtlpExporter` and in development use `aspire-dashboard` OTLP endpoint address as `OTLPExporterEndpoint` and in production we can use `otel-collector` OTLP endpoint address
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            metrics.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.AspireDashboardOTLPOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseGrafanaExporter)
        {
            // https://github.com/grafana/grafana-opentelemetry-dotnet/blob/main/docs/configuration.md#aspnet-core
            // https://github.com/grafana/grafana-opentelemetry-dotnet/
            // https://github.com/grafana/grafana-opentelemetry-dotnet/blob/main/docs/configuration.md#sending-to-an-agent-or-collector-via-otlp
            // https://grafana.com/docs/grafana-cloud/monitor-applications/application-observability/instrument/dotnet/
            metrics.UseGrafana();
        }
    }

    private static void AddLoggingExporters(
        this OpenTelemetryLoggerOptions openTelemetryLoggerOptions,
        ObservabilityOptions observabilityOptions
    )
    {
        if (observabilityOptions.UseOTLPExporter)
        {
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            openTelemetryLoggerOptions.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(observabilityOptions.OTLPOptions.OTLPGrpcExporterEndpoint);
                options.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseAspireOTLPExporter)
        {
            // we can just one `AddOtlpExporter` and in development use `aspire-dashboard` OTLP endpoint address as `OTLPExporterEndpoint` and in production we can use `otel-collector` OTLP endpoint address
            ArgumentNullException.ThrowIfNull(observabilityOptions.OTLPOptions);
            openTelemetryLoggerOptions.AddOtlpExporter(x =>
            {
                x.Endpoint = new Uri(observabilityOptions.AspireDashboardOTLPOptions.OTLPGrpcExporterEndpoint);
                x.Protocol = OtlpExportProtocol.Grpc;
            });
        }

        if (observabilityOptions.UseGrafanaExporter)
        {
            // https://github.com/grafana/grafana-opentelemetry-dotnet/
            // https://github.com/grafana/grafana-opentelemetry-dotnet/blob/main/docs/configuration.md#aspnet-core
            // https://grafana.com/docs/grafana-cloud/monitor-applications/application-observability/instrument/dotnet/
            openTelemetryLoggerOptions.UseGrafana();
        }

        if (observabilityOptions.UseConsoleExporter)
        {
            openTelemetryLoggerOptions.AddConsoleExporter();
        }
    }

    private static WebApplicationBuilder AddCoreDiagnostics(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<CommandHandlerActivity>();
        builder.Services.AddTransient<CommandHandlerMetrics>();
        builder.Services.AddTransient<QueryHandlerActivity>();
        builder.Services.AddTransient<QueryHandlerMetrics>();

        return builder;
    }
}