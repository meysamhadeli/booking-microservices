using System.Text;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SpectreConsole;

namespace BuildingBlocks.Logging
{
    public static class Extensions
    {
        public static WebApplicationBuilder AddCustomSerilog(this WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var logOptions = context.Configuration.GetSection(nameof(LogOptions)).Get<LogOptions>();
                var appOptions = context.Configuration.GetSection(nameof(AppOptions)).Get<AppOptions>();


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

                if (logOptions.Elastic is { Enabled: true })
                {
                    loggerConfiguration.WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(logOptions.Elastic.ElasticServiceUrl))
                        {
                            AutoRegisterTemplate = true,
                            IndexFormat = $"{appOptions.Name}-{environment?.ToLower()}"
                        });
                }


                if (logOptions?.Sentry is {Enabled: true})
                {
                    var minimumBreadcrumbLevel = Enum.TryParse<LogEventLevel>(logOptions.Level, true, out var minBreadcrumbLevel)
                        ? minBreadcrumbLevel
                        : LogEventLevel.Information;

                    var minimumEventLevel = Enum.TryParse<LogEventLevel>(logOptions.Sentry.MinimumEventLevel, true, out var minEventLevel)
                        ? minEventLevel
                        : LogEventLevel.Error;

                    loggerConfiguration.WriteTo.Sentry(o =>
                    {
                        o.Dsn = logOptions.Sentry.Dsn;
                        o.MinimumBreadcrumbLevel = minimumBreadcrumbLevel;
                        o.MinimumEventLevel = minimumEventLevel;
                    });
                }

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
