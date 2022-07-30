using BuildingBlocks.Core;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<PersistMessageOptions>()
            .Bind(configuration.GetSection(nameof(PersistMessageOptions)))
            .ValidateDataAnnotations();

        var persistMessageOptions = services.GetOptions<PersistMessageOptions>("PersistMessageOptions");

        services.AddDbContext<PersistMessageDbContext>(options =>
            options.UseSqlServer(persistMessageOptions.ConnectionString,
                x => x.MigrationsAssembly(typeof(PersistMessageDbContext).Assembly.GetName().Name)));

        services.AddScoped<IPersistMessageDbContext>(provider => provider.GetService<PersistMessageDbContext>());

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();
        services.AddHostedService<PersistMessageBackgroundService>();

        return services;
    }
}
