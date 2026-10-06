using Booking.Data;
using Griffin.Core;
using Griffin.EventStoreDB;
using Griffin.HealthCheck;
using Griffin.Jwt;
using Griffin.Mapster;
using Griffin.Wolverine;
using Griffin.Mongo;
using Griffin.OpenApi;
using Griffin.OpenTelemetryCollector;
using Griffin.ProblemDetails;
using Griffin.Web;
using Figgle;
using Figgle.Fonts;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDefaults;

namespace Booking.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.AddServiceDefaults();

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, BookingEventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));

        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddMongoDbContext<BookingReadDbContext>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddJwt();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddCustomMediatR();
        builder.Services.AddValidatorsFromAssembly(typeof(BookingRoot).Assembly);
        builder.Services.AddProblemDetails();
        builder.Services.AddCustomMapster(typeof(BookingRoot).Assembly);
        builder.AddCustomWolverine(env, TransportType.RabbitMq, nameof(Booking), typeof(BookingRoot).Assembly);
        builder.Services.AddTransient<AuthHeaderHandler>();

        // ref: https://github.com/oskardudycz/EventSourcing.NetCore/tree/main/Sample/EventStoreDB/ECommerce
        builder.Services.AddEventStore(configuration, typeof(BookingRoot).Assembly)
            .AddEventStoreDBSubscriptionToAll();

        builder.Services.AddGrpcClients();

        return builder;
    }


    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var env = app.Environment;
        var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseServiceDefaults();

        app.UseCustomProblemDetails();
        app.UseCorrelationId();
        app.MapGet("/", async x =>
        {
            x.Response.ContentType = "text/plain;charset=utf-8";
            await x.Response.WriteAsync(appOptions.Name);
        });

        if (env.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        return app;
    }
}