using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SpectreConsole;

namespace BuildingBlocks.Logging;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            var loggOptions = context.Configuration.GetSection(nameof(LogOptions)).Get<LogOptions>();

            var logLevel = Enum.TryParse<LogEventLevel>(loggOptions.Level, true, out var level)
                ? level
                : LogEventLevel.Information;

            loggerConfiguration.WriteTo.Console()
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
