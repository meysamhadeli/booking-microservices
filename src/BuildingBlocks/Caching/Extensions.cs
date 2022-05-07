using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Caching;

public static class Extensions
{
    public static IServiceCollection AddCachingRequest(this IServiceCollection services,
        IList<Assembly> assembliesToScan, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        // ICacheRequest discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan ?? AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(ICacheRequest)),
                false)
            .AsImplementedInterfaces()
            .WithLifetime(lifetime));

        // IInvalidateCacheRequest discovery and registration
        services.Scan(scan => scan
            .FromAssemblies(assembliesToScan ?? AppDomain.CurrentDomain.GetAssemblies())
            .AddClasses(classes => classes.AssignableTo(typeof(IInvalidateCacheRequest)),
                false)
            .AsImplementedInterfaces()
            .WithLifetime(lifetime));

        return services;
    }
}
