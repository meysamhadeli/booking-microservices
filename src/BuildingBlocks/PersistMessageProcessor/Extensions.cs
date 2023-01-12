using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this IServiceCollection services)
    {
        services.AddValidateOptions<PersistMessageOptions>();

        services.AddDbContext<PersistMessageDbContext>(options =>
        {
            var persistMessageOptions = services.GetOptions<PersistMessageOptions>(nameof(PersistMessageOptions));

            options.UseSqlServer(persistMessageOptions.ConnectionString,
                x => x.MigrationsAssembly(typeof(PersistMessageDbContext).Assembly.GetName().Name));
        });

        services.AddScoped<IPersistMessageDbContext>(provider => provider.GetService<PersistMessageDbContext>());

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();
        services.AddHostedService<PersistMessageBackgroundService>();

        return services;
    }
}
