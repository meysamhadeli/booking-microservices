using System.Reflection;
using Woo.Core.Exception;
using Woo.Web;
using Humanizer;
using JasperFx.CodeGeneration.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using global::Wolverine;
using global::Wolverine.EntityFrameworkCore;
using global::Wolverine.ErrorHandling;
using global::Wolverine.Postgresql;
using global::Wolverine.RabbitMQ;

namespace Woo.Wolverine;

public static class Extensions
{
    public static WebApplicationBuilder AddCustomWolverine(
        this WebApplicationBuilder builder,
        IWebHostEnvironment env,
        TransportType transportType,
        string? persistenceConnectionName = null,
        params Assembly[] assemblies
    )
    {
        builder.Services.AddValidateOptions<RabbitMqOptions>();

        builder.Host.UseWolverine(options =>
        {
            options.ServiceLocationPolicy = ServiceLocationPolicy.AllowedButWarn;

            options.Discovery.IncludeAssembly(typeof(DurableInternalCommand).Assembly);
            options.Policies.RegisterInteropMessageAssembly(typeof(DurableInternalCommand).Assembly);

            var persistenceConnectionString = ResolvePersistenceConnectionString(builder.Configuration, persistenceConnectionName);

            options.PersistMessagesWithPostgresql(AddRecommendedKeepAliveSettings(persistenceConnectionString));
            options.UseEntityFrameworkCoreTransactions();
            options.Policies.UseDurableInboxOnAllListeners();
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();
            options.Policies.UseDurableLocalQueues();
            options.Durability.InboxStaleTime = TimeSpan.FromMinutes(5);
            options.Durability.OutboxStaleTime = TimeSpan.FromMinutes(5);

            foreach (var assembly in assemblies)
            {
                options.Discovery.IncludeAssembly(assembly);
                options.Policies.RegisterInteropMessageAssembly(assembly);
            }

            options.Policies.ConventionalLocalRoutingIsAdditive();
            options.Policies.OnException<ValidationException>().MoveToErrorQueue();
            options.Policies.OnException<global::System.Exception>()
                .RetryWithCooldown(
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(400),
                    TimeSpan.FromMilliseconds(800));

            switch (transportType)
            {
                case TransportType.RabbitMq:
                    var configuration = builder.Configuration;
                    var aspireConnectionString = configuration.GetConnectionString("rabbitmq");

                    if (!string.IsNullOrWhiteSpace(aspireConnectionString))
                    {
                        options.UseRabbitMqUsingNamedConnection("rabbitmq")
                            .AutoProvision()
                            .UseConventionalRouting();
                    }
                    else
                    {
                        var rabbitMqOptions = builder.Services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

                        ArgumentNullException.ThrowIfNull(rabbitMqOptions);

                        var uri = new Uri(
                            $"amqp://{Uri.EscapeDataString(rabbitMqOptions.UserName)}:{Uri.EscapeDataString(rabbitMqOptions.Password)}@{rabbitMqOptions.HostName}:{rabbitMqOptions.Port ?? 5672}/");

                        options.UseRabbitMq(uri)
                            .AutoProvision()
                            .UseConventionalRouting();
                    }

                    break;

                case TransportType.InMemory:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(transportType),
                        transportType,
                        message: null);
            }

            if (env.IsEnvironment("test"))
            {
                options.StubAllExternalTransports();
            }
        });

        return builder;
    }

    private static string ResolvePersistenceConnectionString(IConfiguration configuration, string? connectionName)
    {
        var connectionString = ResolveNamedConnectionString(configuration, connectionName)
                               ?? configuration.GetConnectionString("wolverine")
                               ?? configuration.GetConnectionString("postgres")
                               ?? configuration["PostgresOptions:ConnectionString"];

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return connectionString;
    }

    private static string? ResolveNamedConnectionString(IConfiguration configuration, string? connectionName)
    {
        if (string.IsNullOrWhiteSpace(connectionName))
        {
            return null;
        }

        return configuration.GetConnectionString(connectionName)
               ?? configuration.GetConnectionString(connectionName.Kebaberize())
               ?? configuration[$"PostgresOptions:ConnectionString:{connectionName}"]
               ?? configuration[$"PostgresOptions:ConnectionString:{connectionName.Kebaberize()}"];
    }

    private static string AddRecommendedKeepAliveSettings(string connectionString)
    {
        if (connectionString.Contains("Keepalive=", StringComparison.OrdinalIgnoreCase))
        {
            return connectionString;
        }

        return connectionString.TrimEnd(';') + ";Keepalive=30;Tcp Keepalive=true";
    }
}