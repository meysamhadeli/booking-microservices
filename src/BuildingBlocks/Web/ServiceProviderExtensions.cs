using Microsoft.Extensions.Hosting;

namespace BuildingBlocks.Web;

public static class ServiceProviderExtensions
{
    public static async Task StartTestHostedServices(
        this IServiceProvider serviceProvider,
        Type[] hostedServiceTypes,
        CancellationToken cancellationToken = default)
    {
        foreach (var hostedServiceType in hostedServiceTypes)
        {
            if (serviceProvider.GetService(hostedServiceType) is IHostedService hostedService)
                await hostedService.StartAsync(cancellationToken);
        }
    }

    public static async Task StopTestHostedServices(
        this IServiceProvider serviceProvider,
        Type[] hostedServiceTypes,
        CancellationToken cancellationToken = default)
    {
        foreach (var hostedServiceType in hostedServiceTypes)
        {
            if (serviceProvider.GetService(hostedServiceType) is IHostedService hostedService)
                await hostedService.StopAsync(cancellationToken);
        }
    }
}
