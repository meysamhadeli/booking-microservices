using System.Reflection;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SpectreConsole;

namespace BuildingBlocks.Logging;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var loggOptions = context.Configuration.GetSection(nameof(LogOptions)).Get<LogOptions>();
            var appOptions = context.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();

            var logLevel = Enum.TryParse<LogEventLevel>(loggOptions.Level, true, out var level)
                ? level
                : LogEventLevel.Information;

            loggerConfiguration
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(loggOptions.ElasticUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat =
                        $"{appOptions.Name}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                })
                .WriteTo.SpectreConsole(loggOptions.LogTemplate, logLevel)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .Enrich.WithSpan()
                .Enrich.WithExceptionDetails()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }
}
