using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.Exception;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Jwt;
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Passenger.Data;
using Passenger.GrpcServer.Services;

namespace Passenger.Extensions.Infrastructure;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.Services.AddCustomHealthCheck();

        builder.AddCustomObservability();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
                                                     {
                                                         http.AddStandardResilienceHandler(options =>
                                                        {
                                                            var timeSpan = TimeSpan.FromMinutes(1);
                                                            options.CircuitBreaker.SamplingDuration = timeSpan * 2;
                                                            options.TotalRequestTimeout.Timeout = timeSpan * 3;
                                                            options.Retry.MaxRetryAttempts = 3;
                                                        });

                                                         // Turn on service discovery by default
                                                         http.AddServiceDiscovery();
                                                     });


        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, PassengerEventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.Configure<ApiBehaviorOptions>(
            options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddPersistMessageProcessor(nameof(PersistMessage));
        builder.AddCustomDbContext<PassengerDbContext>(nameof(Passenger));
        builder.AddMongoDbContext<PassengerReadDbContext>();

        builder.Services.AddJwt();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddCustomMediatR();
        builder.Services.AddValidatorsFromAssembly(typeof(PassengerRoot).Assembly);
        builder.Services.AddProblemDetails();
        builder.Services.AddCustomMapster(typeof(PassengerRoot).Assembly);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCustomMassTransit(env, TransportType.RabbitMq, typeof(PassengerRoot).Assembly);

        builder.Services.AddGrpc(
            options =>
            {
                options.Interceptors.Add<GrpcExceptionInterceptor>();
            });

        return builder;
    }


    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var env = app.Environment;
        var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCustomHealthCheck();
        app.UseCustomObservability();

        app.UseCustomProblemDetails();

        app.UseCorrelationId();
        app.UseMigration<PassengerDbContext>();
        app.MapGrpcService<PassengerGrpcServices>();
        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (env.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        return app;
    }
}