using BuildingBlocks.Core;
using BuildingBlocks.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddTransient<IEventMapper, EventMapper>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        return services;
    }
}
