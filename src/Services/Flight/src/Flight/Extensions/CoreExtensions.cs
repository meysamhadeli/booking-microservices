using BuildingBlocks.Core;
using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flight.Extensions;

public static class CoreExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppOptions>()
            .Bind(configuration.GetSection(nameof(AppOptions)))
            .ValidateDataAnnotations();

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddTransient<IEventMapper, EventMapper>();
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        return services;
    }
}
