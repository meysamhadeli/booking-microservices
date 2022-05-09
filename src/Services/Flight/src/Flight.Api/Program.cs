using System.Reflection;
using BuildingBlocks.Caching;
using BuildingBlocks.Domain;
using BuildingBlocks.EFCore;
using BuildingBlocks.Exception;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.Swagger;
using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using Figgle;
using Flight;
using Flight.Data;
using Flight.Data.Seed;
using Flight.Extensions;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder);

var app = builder.Build();

ConfigureApplication(app);

app.Run();

static void RegisterServices(WebApplicationBuilder builder)
{
    var configuration = builder.Configuration;
    var services = builder.Services;

    var appOptions = services.GetOptions<AppOptions>("AppOptions");
    Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

    builder.AddCustomSerilog();

    services.AddCustomDbContext<FlightDbContext>(configuration, typeof(FlightRoot).Assembly);
    services.AddScoped<IDataSeeder, FlightDataSeeder>();
    services.AddJwt();
    services.AddControllers();
    services.AddCustomSwagger(builder.Configuration, typeof(FlightRoot).Assembly);
    services.AddCustomVersioning();
    services.AddCustomMediatR();
    services.AddValidatorsFromAssembly(typeof(FlightRoot).Assembly);
    services.AddCustomProblemDetails();
    services.AddCustomMapster(typeof(FlightRoot).Assembly);
    services.AddHttpContextAccessor();
    services.AddTransient<IEventMapper, EventMapper>();
    services.AddCustomMassTransit(typeof(FlightRoot).Assembly);
    services.AddCustomOpenTelemetry();
    services.AddRouting(options => options.LowercaseUrls = true);

    services.AddGrpc(options =>
    {
        options.Interceptors.Add<GrpcExceptionInterceptor>();
    });

    services.AddMagicOnion();

    SnowFlakIdGenerator.Configure(1);

    services.AddCachingRequest(new List<Assembly> {typeof(FlightRoot).Assembly});

    services.AddEasyCaching(options => { options.UseInMemory(configuration, "mem"); });
}

static void ConfigureApplication(WebApplication app)
{
    var appOptions = app.GetOptions<AppOptions>("AppOptions");

    if (app.Environment.IsDevelopment())
    {
        var provider = app.Services.GetService<IApiVersionDescriptionProvider>();
        app.UseCustomSwagger(provider);
    }

    app.UseSerilogRequestLogging();
    app.UseCorrelationId();
    app.UseRouting();
    app.UseHttpMetrics();
    app.UseMigrations();
    app.UseProblemDetails();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapMetrics();
        endpoints.MapMagicOnionService();
    });

    app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));
}

public partial class Program { }
