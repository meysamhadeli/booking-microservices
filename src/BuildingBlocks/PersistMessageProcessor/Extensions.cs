using BuildingBlocks.Web;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this WebApplicationBuilder builder, string? connectionName = "persist-message")
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        builder.Services.AddValidateOptions<PersistMessageOptions>();

        builder.Services.AddDbContext<PersistMessageDbContext>(
            (sp, options) =>
            {
                var aspireConnectionString = builder.Configuration.GetConnectionString(connectionName.Kebaberize());

                var connectionString = aspireConnectionString ?? sp.GetRequiredService<PersistMessageOptions>().ConnectionString;

                ArgumentException.ThrowIfNullOrEmpty(connectionString);

                options.UseNpgsql(
                        connectionString,
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

        builder.Services.AddScoped<IPersistMessageDbContext>(
            provider =>
            {
                var persistMessageDbContext =
                    provider.GetRequiredService<PersistMessageDbContext>();

                persistMessageDbContext.Database.EnsureCreated();
                persistMessageDbContext.CreatePersistMessageTableIfNotExists();

                return persistMessageDbContext;
            });

        builder.Services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();

        builder.Services.AddHostedService<PersistMessageBackgroundService>();

        return builder.Services;
    }
}