using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.Mongo;
using Flight.Data;
using Flight.Data.Seed;
using Flight.GrpcServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Flight.Extensions.Infrastructure;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddFlightModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.AddCustomDbContext<FlightDbContext>(nameof(Flight));
        builder.Services.AddScoped<IDataSeeder, FlightDataSeeder>();
        builder.AddMongoDbContext<FlightReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UseFlightModules(this WebApplication app)
    {
        app.UseMigration<FlightDbContext>();
        app.MapGrpcService<FlightGrpcServices>();

        return app;
    }
}
