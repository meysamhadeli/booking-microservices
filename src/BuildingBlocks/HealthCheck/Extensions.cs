using BuildingBlocks.EFCore;
using BuildingBlocks.Logging;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.Web;
using DotNetCore.CAP.MongoDB;
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
        var appOptions = services.GetOptions<AppOptions>("AppOptions");
        var sqlOptions = services.GetOptions<SqlOptions>("ConnectionStrings");
        var rabbitMqOptions = services.GetOptions<RabbitMqOptions>("RabbitMq");
        var mongoOptions = services.GetOptions<MongoOptions>("MongoOptions");
        var logOptions = services.GetOptions<LogOptions>("LogOptions");

        services.AddHealthChecks()
            .AddSqlServer(sqlOptions.DefaultConnection)
            .AddMongoDb(mongoOptions.ConnectionString)
            .AddRabbitMQ(rabbitConnectionString: $"amqp://{rabbitMqOptions.UserName}:{rabbitMqOptions.Password}@{rabbitMqOptions.HostName}")
            .AddElasticsearch(logOptions.ElasticUri);

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(60); // time in seconds between check
            setup.AddHealthCheckEndpoint($"Basic Health Check - {appOptions.Name}", "/healthz");
        }).AddInMemoryStorage();

        return services;
    }

    public static WebApplication UseCustomHealthCheck(this WebApplication app)
    {
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
