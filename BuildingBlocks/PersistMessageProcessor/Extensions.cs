using BuildingBlocks.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this IServiceCollection services)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddValidateOptions<PersistMessageOptions>();

        services.AddDbContext<PersistMessageDbContext>(
            (sp, options) =>
            {
                var persistMessageOptions = sp.GetRequiredService<PersistMessageOptions>();

                options.UseNpgsql(
                        persistMessageOptions.ConnectionString,
                        dbOptions =>
                        {
                            dbOptions.MigrationsAssembly(
                                typeof(PersistMessageDbContext).Assembly.GetName().Name);
                        })
                    // https://github.com/efcore/EFCore.NamingConventions
                    .UseSnakeCaseNamingConvention();

                // Todo: follow up the issues of .net 9 to use better approach taht will provide by .net!
                options.ConfigureWarnings(
                    w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            });

        services.AddScoped<IPersistMessageDbContext>(
            provider =>
            {
                var persistMessageDbContext =
                    provider.GetRequiredService<PersistMessageDbContext>();

                persistMessageDbContext.Database.EnsureCreated();
                persistMessageDbContext.CreatePersistMessageTableIfNotExists();

                return persistMessageDbContext;
            });

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();

        services.AddHostedService<PersistMessageBackgroundService>();

        return services;
    }
}
