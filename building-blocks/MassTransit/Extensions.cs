using System.Reflection;
using BuildingBlocks.Web;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.MassTransit;

using Exception;

public static class Extensions
{
    public static IServiceCollection AddCustomMassTransit(
        this IServiceCollection services,
        IWebHostEnvironment env,
        TransportType transportType,
        params Assembly[] assembly
    )
    {
        services.AddValidateOptions<RabbitMqOptions>();

        if (env.IsEnvironment("test"))
        {
            services.AddMassTransitTestHarness(
                configure =>
                {
                    SetupMasstransitConfigurations(services, configure, transportType, assembly);
                });
        }
        else
        {
            services.AddMassTransit(
                configure =>
                {
                    SetupMasstransitConfigurations(services, configure, transportType, assembly);
                });
        }

        return services;
    }

    private static void SetupMasstransitConfigurations(
        IServiceCollection services,
        IBusRegistrationConfigurator configure,
        TransportType transportType,
        params Assembly[] assembly
    )
    {
        configure.AddConsumers(assembly);
        configure.AddSagaStateMachines(assembly);
        configure.AddSagas(assembly);
        configure.AddActivities(assembly);

        switch (transportType)
        {
            case TransportType.RabbitMq:
                configure.UsingRabbitMq(
                    (context, configurator) =>
                    {
                        var rabbitMqOptions =
                            services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));

                        configurator.Host(
                            rabbitMqOptions?.HostName,
                            rabbitMqOptions?.Port ?? 5672,
                            "/",
                            h =>
                            {
                                h.Username(rabbitMqOptions?.UserName);
                                h.Password(rabbitMqOptions?.Password);
                            });

                        configurator.ConfigureEndpoints(context);

                        configurator.UseMessageRetry(AddRetryConfiguration);
                    });

                break;
            case TransportType.InMemory:
                configure.UsingInMemory(
                    (context, configurator) =>
                    {
                        configurator.ConfigureEndpoints(context);
                        configurator.UseMessageRetry(AddRetryConfiguration);
                    });

                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(transportType),
                    transportType,
                    message: null);
        }
    }

    private static void AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator.Exponential(
                3,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMinutes(120),
                TimeSpan.FromMilliseconds(200))
            .Ignore<
                ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit
    }
}
