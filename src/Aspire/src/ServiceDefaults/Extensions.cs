using BuildingBlocks.HealthCheck;
using BuildingBlocks.OpenTelemetryCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        builder.Services.AddCustomHealthCheck();
        builder.AddCustomObservability();
        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
                                                     {
                                                         http.AddStandardResilienceHandler(options =>
                                                             {
                                                                 var timeSpan = TimeSpan.FromMinutes(1);
                                                                 options.CircuitBreaker.SamplingDuration = timeSpan * 2;
                                                                 options.TotalRequestTimeout.Timeout = timeSpan * 3;
                                                                 options.Retry.MaxRetryAttempts = 3;
                                                             });

                                                         // Turn on service discovery by default
                                                         http.AddServiceDiscovery();
                                                     });

        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseCustomHealthCheck();
        app.UseCustomObservability();

        return app;
    }
}