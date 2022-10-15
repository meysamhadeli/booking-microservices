using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace BuildingBlocks.OpenTelemetry;

public static class Extensions
{
    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetryTracing(builder => builder
            .AddGrpcClientInstrumentation()
            .AddMassTransitInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(services.GetOptions<AppOptions>("AppOptions").Name))
            .AddJaegerExporter());

        return services;
    }
}
