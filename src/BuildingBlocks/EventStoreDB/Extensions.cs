using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EventStoreDB;

using Web;

public static class Extensions
{
    // ref: https://github.com/oskardudycz/EventSourcing.NetCore/tree/main/Sample/EventStoreDB/ECommerce
    public static IServiceCollection AddEventStore(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies
    )
    {
        services.AddValidateOptions<EventStoreOptions>();

        var assembliesToScan = assemblies.Length > 0 ? assemblies : new[] { Assembly.GetEntryAssembly()! };

        return services
            .AddEventStoreDB(configuration)
            .AddProjections(assembliesToScan);
    }
}
