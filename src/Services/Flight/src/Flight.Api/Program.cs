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
using BuildingBlocks.Mongo;
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
var configuration = builder.Configuration;
var env = builder.Environment;

var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));


builder.Services.AddTransient<IBusPublisher, BusPublisher>();
builder.Services.AddCustomDbContext<FlightDbContext>(configuration, typeof(FlightRoot).Assembly);
builder.Services.AddMongoDbContext<FlightReadDbContext>(configuration);

builder.Services.AddScoped<IDataSeeder, FlightDataSeeder>();
builder.AddCustomSerilog();
builder.Services.AddJwt();
builder.Services.AddControllers();
builder.Services.AddCustomSwagger(builder.Configuration, typeof(FlightRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.Services.AddCustomMediatR();
builder.Services.AddValidatorsFromAssembly(typeof(FlightRoot).Assembly);
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomMapster(typeof(FlightRoot).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IEventMapper, EventMapper>();
builder.Services.AddCustomMassTransit(typeof(FlightRoot).Assembly, env);
builder.Services.AddCustomOpenTelemetry();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

builder.Services.AddMagicOnion();

SnowFlakIdGenerator.Configure(1);

builder.Services.AddCachingRequest(new List<Assembly>
{
    typeof(FlightRoot).Assembly
});

builder.Services.AddEasyCaching(options => { options.UseInMemory(configuration, "mem"); });

var app = builder.Build();

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
app.Run();

public partial class Program {}
