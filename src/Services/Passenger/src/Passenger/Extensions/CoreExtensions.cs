using BuildingBlocks.Core;
using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Passenger.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        configuration.GetSection(nameof(AppOptions)).Bind(nameof(AppOptions));

        services.AddOptions<AppOptions>()
            .Bind(configuration.GetSection(nameof(AppOptions)))
            .ValidateDataAnnotations();

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddTransient<IEventMapper, EventMapper>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        return services;
    }
}
