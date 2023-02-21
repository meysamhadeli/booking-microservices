using System.Reflection;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Humanizer;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.MassTransit;

using Exception;

public static class Extensions
{
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services,
        IWebHostEnvironment env, Assembly assembly)
    {
        services.AddValidateOptions<RabbitMqOptions>();

        if (env.IsEnvironment("test"))
        {
            services.AddMassTransitTestHarness(configure =>
            {
                SetupMasstransitConfigurations(services, configure, assembly);
            });
        }
        else
        {
            services.AddMassTransit(configure => { SetupMasstransitConfigurations(services, configure, assembly); });
        }

        return services;
    }

    private static void SetupMasstransitConfigurations(IServiceCollection services,
        IBusRegistrationConfigurator configure, Assembly assembly)
    {
        configure.AddConsumers(assembly);

        configure.UsingRabbitMq((context, configurator) =>
        {
            var rabbitMqOptions = services.GetOptions<RabbitMqOptions>(nameof(RabbitMqOptions));


            configurator.Host(rabbitMqOptions?.HostName, rabbitMqOptions?.Port ?? 5672, "/", h =>
            {
                h.Username(rabbitMqOptions?.UserName);
                h.Password(rabbitMqOptions?.Password);
            });


            var integrationEventRootAssemblies = Assembly.GetAssembly(typeof(IIntegrationEvent));

            var types = integrationEventRootAssemblies?.GetTypes()
                .Where(x => x.IsAssignableTo(typeof(IIntegrationEvent))
                            && !x.IsInterface
                            && !x.IsAbstract
                            && !x.IsGenericType)?.ToList();

            if (types != null && types.Any())
            {
                foreach (var type in types)
                {
                    var consumers = assembly?.GetTypes()
                        .Where(x => x.IsAssignableTo(typeof(IConsumer<>).MakeGenericType(type))).ToList();

                    if (consumers != null && consumers.Any())
                        configurator.ReceiveEndpoint(
                            string.IsNullOrEmpty(rabbitMqOptions.ExchangeName)
                                ? type.Name.Underscore()
                                : $"{rabbitMqOptions.ExchangeName}_{type.Name.Underscore()}", e =>
                            {
                                e.UseConsumeFilter(typeof(ConsumeFilter<>), context); //generic filter

                                foreach (var consumer in consumers)
                                {
                                    //ref: https://masstransit-project.com/usage/exceptions.html#retry
                                    //ref: https://markgossa.com/2022/06/masstransit-exponential-back-off.html
                                    configurator.UseMessageRetry(r => AddRetryConfiguration(r));

                                    configurator.ConfigureEndpoints(context, x => x.Exclude(consumer));
                                    var methodInfo = typeof(DependencyInjectionReceiveEndpointExtensions)
                                        .GetMethods()
                                        .Where(x => x.GetParameters()
                                            .Any(p => p.ParameterType == typeof(IServiceProvider)))
                                        .FirstOrDefault(x => x.Name == "Consumer" && x.IsGenericMethod);

                                    var generic = methodInfo?.MakeGenericMethod(consumer);
                                    generic?.Invoke(e, new object[] { e, context, null });
                                }
                            });
                }
            }
        });
    }

    private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator.Exponential(
                3,
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMinutes(120),
                TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit

        return retryConfigurator;
    }
}
