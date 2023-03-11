using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

public static class Extensions
{
    public static IServiceCollection AddPersistMessageProcessor(this IServiceCollection services)
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
                .UseSnakeCaseNamingConvention();;
        });

        services.AddScoped<IPersistMessageDbContext>(provider => provider.GetService<PersistMessageDbContext>());

        services.AddScoped<IPersistMessageProcessor, PersistMessageProcessor>();
        services.AddHostedService<PersistMessageBackgroundService>();

        return services;
    }

    public static IApplicationBuilder UseMigrationPersistMessage<TContext>(this IApplicationBuilder app, IWebHostEnvironment env)
        where TContext : DbContext, IPersistMessageDbContext
    {
        using var scope = app.ApplicationServices.CreateScope();

        var persistMessageContext = scope.ServiceProvider.GetRequiredService<PersistMessageDbContext>();
        persistMessageContext.Database.Migrate();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        context.Database.Migrate();

        return app;
    }
}
