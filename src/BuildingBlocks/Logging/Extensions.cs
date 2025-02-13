using System.Globalization;
using System.Text;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SpectreConsole;

namespace BuildingBlocks.Logging
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                var logOptions = context.Configuration.GetSection(nameof(LogOptions)).Get<LogOptions>();

                var logLevel = Enum.TryParse<LogEventLevel>(logOptions.Level, true, out var level)
                    ? level
                    : LogEventLevel.Information;

                loggerConfiguration
                    .MinimumLevel.Is(logLevel)
                    .WriteTo.SpectreConsole(logOptions.LogTemplate, logLevel)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    // Only show ef-core information in error level
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                    // Filter out ASP.NET Core infrastructure logs that are Information and below
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .ReadFrom.Configuration(context.Configuration);

                if (logOptions.File is { Enabled: true })
                {
                    var root = env.ContentRootPath;
                    Directory.CreateDirectory(Path.Combine(root, "logs"));

                    var path = string.IsNullOrWhiteSpace(logOptions.File.Path) ? "logs/.txt" : logOptions.File.Path;
                    if (!Enum.TryParse<RollingInterval>(logOptions.File.Interval, true, out var interval))
                    {
                        interval = RollingInterval.Day;
                    }

                    loggerConfiguration.WriteTo.File(path, rollingInterval: interval, encoding: Encoding.UTF8, outputTemplate: logOptions.LogTemplate);
                }
            });

            return builder;
        }
    }
}
