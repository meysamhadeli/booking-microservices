using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.Mapster;
using BuildingBlocks.Mongo;
using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Passenger.Data;
using Passenger.GrpcServer.Services;

namespace Passenger.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddPassengerModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.AddMinimalEndpoints(assemblies: typeof(PassengerRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(PassengerRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(PassengerRoot).Assembly);
        builder.AddCustomDbContext<PassengerDbContext>(nameof(Passenger));
        builder.AddMongoDbContext<PassengerReadDbContext>();

        builder.Services.AddCustomMediatR();

        return builder;
    }


    public static WebApplication UsePassengerModules(this WebApplication app)
    {
        app.UseMigration<PassengerDbContext>();
        app.MapGrpcService<PassengerGrpcServices>();

        return app;
    }
}
