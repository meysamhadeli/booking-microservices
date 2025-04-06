using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Mongo
{
    using Web;

    public static class Extensions
    {
        public static IServiceCollection AddMongoDbContext<TContext>(
            this WebApplicationBuilder builder, Action<MongoOptions>? configurator = null)
        where TContext : MongoDbContext
        {
            return builder.Services.AddMongoDbContext<TContext, TContext>(builder.Configuration, configurator);
        }

        public static IServiceCollection AddMongoDbContext<TContextService, TContextImplementation>(
            this IServiceCollection services, IConfiguration configuration, Action<MongoOptions>? configurator = null)
        where TContextService : IMongoDbContext
        where TContextImplementation : MongoDbContext, TContextService
        {
            services.Configure<MongoOptions>(configuration.GetSection(nameof(MongoOptions)));

            if (configurator is { })
            {
                services.Configure(nameof(MongoOptions), configurator);
            }
            else
            {
                services.AddValidateOptions<MongoOptions>();
            }

            services.AddScoped(typeof(TContextService), typeof(TContextImplementation));
            services.AddScoped(typeof(TContextImplementation));

            services.AddScoped<IMongoDbContext>(sp => sp.GetRequiredService<TContextService>());

            services.AddTransient(typeof(IMongoRepository<,>), typeof(MongoRepository<,>));
            services.AddTransient(typeof(IMongoUnitOfWork<>), typeof(MongoUnitOfWork<>));

            return services;
        }
    }
}
