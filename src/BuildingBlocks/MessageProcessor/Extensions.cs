using BuildingBlocks.Core;
using BuildingBlocks.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.MessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PersistMessageOptions>()
            .Bind(configuration.GetSection(nameof(PersistMessageOptions)))
            .ValidateDataAnnotations();

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();
        services.AddHostedService<PersistMessageBackgroundService>();

        return services;
    }
}
