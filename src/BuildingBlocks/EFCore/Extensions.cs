using System.Linq.Expressions;
using BuildingBlocks.Core.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.EFCore;

public static class Extensions
{
    public static IServiceCollection AddCustomDbContext<TContext>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TContext : DbContext, IDbContext
    {
        services.AddDbContext<TContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly(typeof(TContext).Assembly.GetName().Name)));

        services.AddScoped<IDbContext>(provider => provider.GetService<TContext>());

        return services;
    }

    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app)
        where TContext : DbContext, IDbContext
    {
        MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();
        SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();

        return app;
    }


    // ref: https://github.com/pdevito3/MessageBusTestingInMemHarness/blob/main/RecipeManagement/src/RecipeManagement/Databases/RecipesDbContext.cs
    public static void FilterSoftDeletedProperties(this ModelBuilder modelBuilder)
    {
        Expression<Func<IAggregate, bool>> filterExpr = e => !e.IsDeleted;
        foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes()
                     .Where(m => m.ClrType.IsAssignableTo(typeof(IEntity))))
        {
            // modify expression to handle correct child type
            var parameter = Expression.Parameter(mutableEntityType.ClrType);
            var body = ReplacingExpressionVisitor
                .Replace(filterExpr.Parameters.First(), parameter, filterExpr.Body);
            var lambdaExpression = Expression.Lambda(body, parameter);

            // set filter
            mutableEntityType.SetQueryFilter(lambdaExpression);
        }
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider)
        where TContext : DbContext, IDbContext
    {
        using var scope = serviceProvider.CreateScope();

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
