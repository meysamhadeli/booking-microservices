using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.PersistMessageProcessor;

using EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app, IWebHostEnvironment env)
        where TContext : DbContext, IPersistMessageDbContext
    {
        MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();

        if (!env.IsEnvironment("test"))
        {
            SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        }

        return app;
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider)
        where TContext : DbContext, IPersistMessageDbContext
    {
        using var scope = serviceProvider.CreateScope();

        var persistMessageContext = scope.ServiceProvider.GetRequiredService<PersistMessageDbContext>();
        await persistMessageContext.Database.MigrateAsync();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}
