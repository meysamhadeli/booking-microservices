using System.Reflection;
using BuildingBlocks.Caching;
using BuildingBlocks.EFCore;
using BuildingBlocks.Exception;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using Figgle;
using Flight;
using Flight.Data;
using Flight.Data.Seed;
using Flight.Extensions;
using Flight.Flights.Features.CreateFlight.Endpoints.V1;
using Flight.GrpcServer.Services;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

builder.Services.AddCustomDbContext<FlightDbContext>(configuration);
builder.Services.AddScoped<IDataSeeder, FlightDataSeeder>();

builder.Services.AddMongoDbContext<FlightReadDbContext>(configuration);
builder.Services.AddPersistMessage(configuration);

builder.AddCustomSerilog(env);
builder.Services.AddCore();
builder.Services.AddJwt();
builder.Services.AddCustomSwagger(configuration, typeof(FlightRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.Services.AddCustomMediatR();
builder.Services.AddValidatorsFromAssembly(typeof(FlightRoot).Assembly);
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomMapster(typeof(FlightRoot).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomMassTransit(typeof(FlightRoot).Assembly, env);
builder.Services.AddCustomOpenTelemetry();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCustomHealthCheck();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

SnowFlakIdGenerator.Configure(1);

builder.Services.AddCachingRequest(new List<Assembly> {typeof(FlightRoot).Assembly});

builder.Services.AddEasyCaching(options => { options.UseInMemory(configuration, "mem"); });

builder.AddMinimalEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseSerilogRequestLogging();
app.UseCorrelationId();
app.UseRouting();
app.UseHttpMetrics();
app.UseMigration<FlightDbContext>(env);
app.UseProblemDetails();
app.UseHttpsRedirection();
app.UseCustomHealthCheck();
app.UseAuthentication();
app.UseAuthorization();


app.MapMinimalEndpoints();

app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
    endpoints.MapGrpcService<FlightGrpcServices>();
});

app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));
app.Run();

namespace Flight.Api
{
    public partial class Program
    {
    }
}
