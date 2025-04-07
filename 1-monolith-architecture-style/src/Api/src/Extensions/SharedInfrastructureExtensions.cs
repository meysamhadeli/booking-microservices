using System.Threading.RateLimiting;
using BookingMonolith;
using BookingMonolith.Booking.Data;
using BookingMonolith.Flight.Data;
using BookingMonolith.Flight.Data.Seed;
using BookingMonolith.Identity.Data;
using BookingMonolith.Identity.Data.Seed;
using BookingMonolith.Identity.Extensions.Infrastructure;
using BookingMonolith.Passenger.Data;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.EventStoreDB;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.OpenApi;
using BuildingBlocks.OpenTelemetryCollector;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.ProblemDetails;
using BuildingBlocks.Web;
using Figgle;
using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Api.Extensions;

public static class SharedInfrastructureExtensions
{
    public static WebApplicationBuilder AddSharedInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider(
            (context, options) =>
            {
                // Service provider validation
                // ref: https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment() ||
                                         context.HostingEnvironment.IsStaging() ||
                                         context.HostingEnvironment.IsEnvironment("tests");

                options.ValidateOnBuild = true;
            });

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddCustomSerilog(builder.Environment);
        builder.Services.AddJwt();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddPersistMessageProcessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddControllers();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
        builder.Services.AddCustomMediatR();

        builder.Services.AddCustomMassTransit(
            builder.Environment,
            TransportType.InMemory,
            AppDomain.CurrentDomain.GetAssemblies());


        builder.Services.Scan(
            scan => scan
                .FromAssemblyOf<BookingMonolithRoot>()
                .AddClasses(classes => classes.AssignableTo<IEventMapper>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());


        builder.AddMinimalEndpoints(assemblies: typeof(BookingMonolithRoot).Assembly);
        builder.Services.AddValidatorsFromAssembly(typeof(BookingMonolithRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(BookingMonolithRoot).Assembly);

        builder.AddMongoDbContext<FlightReadDbContext>();
        builder.AddMongoDbContext<PassengerReadDbContext>();
        builder.AddMongoDbContext<BookingReadDbContext>();

        builder.AddCustomDbContext<IdentityContext>();
        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        builder.AddCustomIdentityServer();

        builder.Services.Configure<ForwardedHeadersOptions>(
            options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

        builder.AddCustomDbContext<FlightDbContext>();
        builder.Services.AddScoped<IDataSeeder, FlightDataSeeder>();

        builder.AddCustomDbContext<PassengerDbContext>();

        // ref: https://github.com/oskardudycz/EventSourcing.NetCore/tree/main/Sample/EventStoreDB/ECommerce
        builder.Services.AddEventStore(builder.Configuration, typeof(BookingMonolithRoot).Assembly)
            .AddEventStoreDBSubscriptionToAll();

        builder.Services.Configure<ApiBehaviorOptions>(
            options => options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddRateLimiter(
            options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: httpContext.User.Identity?.Name ??
                                          httpContext.Request.Headers.Host.ToString(),
                            factory: partition => new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = true,
                                PermitLimit = 10,
                                QueueLimit = 0,
                                Window = TimeSpan.FromMinutes(1)
                            }));
            });

        builder.AddCustomObservability();
        builder.Services.AddCustomHealthCheck();

        builder.Services.AddEasyCaching(
            options => { options.UseInMemory(builder.Configuration, "mem"); });

        builder.Services.AddProblemDetails();

        return builder;
    }


    public static WebApplication UserSharedInfrastructure(this WebApplication app)
    {
        var appOptions = app.Configuration.GetOptions<AppOptions>(nameof(AppOptions));

        app.UseCustomProblemDetails();
        app.UseCustomObservability();
        app.UseCustomHealthCheck();

        app.UseSerilogRequestLogging(
            options =>
            {
                options.EnrichDiagnosticContext = LogEnrichHelper.EnrichFromRequest;
            });

        app.UseCorrelationId();
        app.UseRateLimiter();
        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (app.Environment.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        app.UseForwardedHeaders();
        app.UseMigration<IdentityContext>();
        app.UseMigration<FlightDbContext>();
        app.UseMigration<PassengerDbContext>();

        app.UseIdentityServer();

        return app;
    }
}
