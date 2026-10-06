using Griffin.Core;
using Griffin.EFCore;
using Griffin.Core.Exception;
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
using Flight.Data;
using Flight.Data.Seed;
using Flight.GrpcServer.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceDefaults;

namespace Flight.Extensions.Infrastructure;


public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.AddServiceDefaults();

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, FlightEventMapper>();

        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddCustomMediatR();
        builder.Services.AddProblemDetails();

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.AddCustomDbContext<FlightDbContext>(nameof(Flight));
        builder.Services.AddScoped<IDataSeeder, FlightDataSeeder>();
        builder.AddMongoDbContext<FlightReadDbContext>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddJwt();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddValidatorsFromAssembly(typeof(FlightRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(FlightRoot).Assembly);
        builder.Services.AddHttpContextAccessor();
        builder.AddCustomWolverine(env, TransportType.RabbitMq, nameof(Flight), typeof(FlightRoot).Assembly);

        builder.Services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        builder.Services.AddEasyCaching(options => { options.UseInMemory(configuration, "mem"); });

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
        app.UseMigration<FlightDbContext>();
        app.MapGrpcService<FlightGrpcServices>();

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