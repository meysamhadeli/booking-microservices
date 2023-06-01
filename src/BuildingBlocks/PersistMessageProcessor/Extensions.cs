using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this IServiceCollection services,
        IWebHostEnvironment env)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddValidateOptions<PersistMessageOptions>();

        services.AddDbContext<PersistMessageDbContext>((sp, options) =>
        {
            var persistMessageOptions = sp.GetRequiredService<PersistMessageOptions>();

            options.UseNpgsql(persistMessageOptions.ConnectionString,
                    dbOptions =>
                    {
                        dbOptions.MigrationsAssembly(typeof(PersistMessageDbContext).Assembly.GetName().Name);
                    })
                // https://github.com/efcore/EFCore.NamingConventions
                .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IPersistMessageDbContext>(provider =>
        {
            var persistMessageDbContext = provider.GetRequiredService<PersistMessageDbContext>();

            persistMessageDbContext.Database.EnsureCreated();
            persistMessageDbContext.CreatePersistMessageTable();

            return persistMessageDbContext;
        });

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();

        if (env.EnvironmentName != "test")
        {
            services.AddHostedService<PersistMessageBackgroundService>();
        }

        return services;
    }
}
