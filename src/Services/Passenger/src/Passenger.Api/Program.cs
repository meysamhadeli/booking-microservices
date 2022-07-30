using BuildingBlocks.Core;
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
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Passenger;
using Passenger.Data;
using Passenger.Extensions;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

builder.Services.AddCustomDbContext<PassengerDbContext>(configuration);
builder.Services.AddMongoDbContext<PassengerReadDbContext>(configuration);
builder.Services.AddPersistMessage(configuration);

builder.AddCustomSerilog();
builder.Services.AddCore();
builder.Services.AddJwt();
builder.Services.AddControllers();
builder.Services.AddCustomSwagger(configuration, typeof(PassengerRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.Services.AddCustomMediatR();
builder.Services.AddValidatorsFromAssembly(typeof(PassengerRoot).Assembly);
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomMapster(typeof(PassengerRoot).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomHealthCheck();
builder.Services.AddCustomMassTransit(typeof(PassengerRoot).Assembly, env);
builder.Services.AddCustomOpenTelemetry();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});
builder.Services.AddMagicOnion();

SnowFlakIdGenerator.Configure(2);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetService<IApiVersionDescriptionProvider>();
    app.UseCustomSwagger(provider);
}

app.UseSerilogRequestLogging();
app.UseMigration<PassengerDbContext>(env);
app.UseCorrelationId();
app.UseRouting();
app.UseHttpMetrics();
app.UseProblemDetails();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCustomHealthCheck();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
    endpoints.MapMagicOnionService();
});

app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

app.Run();

public partial class Program
{
}
