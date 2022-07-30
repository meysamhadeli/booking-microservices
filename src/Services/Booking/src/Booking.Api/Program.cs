using Booking;
using Booking.Configuration;
using Booking.Data;
using Booking.Extensions;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.EventStoreDB;
using BuildingBlocks.EventStoreDB.Projections;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Mongo;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.PersistMessageProcessor.Data;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using Figgle;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
builder.Services.Configure<GrpcOptions>(options => configuration.GetSection("Grpc").Bind(options));

Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

builder.Services.AddPersistMessage(configuration);
builder.Services.AddMongoDbContext<BookingReadDbContext>(configuration);

builder.AddCustomSerilog();
builder.Services.AddCore();
builder.Services.AddJwt();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCustomSwagger(configuration, typeof(BookingRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.Services.AddCustomMediatR();
builder.Services.AddValidatorsFromAssembly(typeof(BookingRoot).Assembly);
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomMapster(typeof(BookingRoot).Assembly);
builder.Services.AddCustomHealthCheck();
builder.Services.AddCustomMassTransit(typeof(BookingRoot).Assembly, env);
builder.Services.AddCustomOpenTelemetry();
builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddMagicOnionClients();

SnowFlakIdGenerator.Configure(3);

// ref: https://github.com/oskardudycz/EventSourcing.NetCore/tree/main/Sample/EventStoreDB/ECommerce
builder.Services.AddEventStore(configuration, typeof(BookingRoot).Assembly)
    .AddEventStoreDBSubscriptionToAll();

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
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseProblemDetails();
app.UseCustomHealthCheck();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

app.Run();

public partial class Program
{
}
