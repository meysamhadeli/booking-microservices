using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry;

using global::OpenTelemetry.Metrics;

public static class Extensions
{
    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddGrpcClientInstrumentation()
                .AddMassTransitInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(services.GetOptions<AppOptions>("AppOptions").Name))
                .AddJaegerExporter())
            .WithMetrics(builder =>
                {
                    builder.AddPrometheusExporter();
                    builder.AddMeter(
                        "Microsoft.AspNetCore.Hosting",
                        "Microsoft.AspNetCore.Server.Kestrel"
                    );
                    builder.AddView("request-duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = new[]
                            {
                                0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                            }
                        });
                }
            );

        return services;
    }
}
