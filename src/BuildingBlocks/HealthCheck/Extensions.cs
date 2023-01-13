using BuildingBlocks.EFCore;
using BuildingBlocks.Logging;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.Web;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BuildingBlocks.HealthCheck;

public static class Extensions
{
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services)
    {
        var healthOptions = services.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (!healthOptions.Enabled) return services;

        var appOptions = services.GetOptions<AppOptions>(nameof(AppOptions));
        var postgresOptions = services.GetOptions<PostgresOptions>(nameof(PostgresOptions));
        var rabbitMqOptions = services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));
        var mongoOptions = services.GetOptions<MongoOptions>(nameof(MongoOptions));
        var logOptions = services.GetOptions<LogOptions>(nameof(LogOptions));

        var healthChecksBuilder = services.AddHealthChecks()
            .AddRabbitMQ(
                rabbitConnectionString:
                $"amqp://{rabbitMqOptions.UserName}:{rabbitMqOptions.Password}@{rabbitMqOptions.HostName}")
            .AddElasticsearch(logOptions.Elastic.ElasticServiceUrl);

        if (mongoOptions.ConnectionString is not null)
            healthChecksBuilder.AddMongoDb(mongoOptions.ConnectionString);

        if (postgresOptions.ConnectionString is not null)
            healthChecksBuilder.AddNpgSql(postgresOptions.ConnectionString);

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(60); // time in seconds between check
            setup.AddHealthCheckEndpoint($"Basic Health Check - {appOptions.Name}", "/healthz");
        }).AddInMemoryStorage();

        return services;
    }

    public static WebApplication UseCustomHealthCheck(this WebApplication app)
    {
        var healthOptions = app.Configuration.GetOptions<HealthOptions>(nameof(HealthOptions));

        if (!healthOptions.Enabled) return app;

        app.UseHealthChecks("/healthz",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                })
            .UseHealthChecksUI(options =>
            {
                options.ApiPath = "/healthcheck";
                options.UIPath = "/healthcheck-ui";
            });

        return app;
    }
}
