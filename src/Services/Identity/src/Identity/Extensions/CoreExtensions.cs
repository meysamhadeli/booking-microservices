using BuildingBlocks.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddTransient<IEventMapper, EventMapper>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        return services;
    }
}
