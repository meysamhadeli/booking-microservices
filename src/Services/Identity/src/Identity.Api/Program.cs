using BuildingBlocks.EFCore;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using Figgle;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Identity;
using Identity.Data;
using Identity.Data.Seed;
using Identity.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var env = builder.Environment;

var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

builder.Services.AddPersistMessage(configuration);
builder.Services.AddCustomDbContext<IdentityContext>(configuration);
builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();
builder.Services.AddCore();
builder.Services.AddControllers();
builder.AddCustomSerilog(env);
builder.Services.AddCustomSwagger(configuration, typeof(IdentityRoot).Assembly);
builder.Services.AddCustomVersioning();
builder.Services.AddCustomMediatR();
builder.Services.AddValidatorsFromAssembly(typeof(IdentityRoot).Assembly);
builder.Services.AddCustomProblemDetails();
builder.Services.AddCustomMapster(typeof(IdentityRoot).Assembly);
builder.Services.AddCustomHealthCheck();

builder.Services.AddCustomMassTransit(typeof(IdentityRoot).Assembly, env);
builder.Services.AddCustomOpenTelemetry();

SnowFlakIdGenerator.Configure(4);

builder.Services.AddIdentityServer(env);

builder.AddMinimalEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseSerilogRequestLogging();
app.UseMigration<IdentityContext>(env);
app.UseCorrelationId();
app.UseRouting();
app.UseHttpMetrics();
app.UseProblemDetails();
app.UseHttpsRedirection();
app.UseCustomHealthCheck();
app.UseAuthentication();
app.UseAuthorization();
app.UseIdentityServer();

app.MapMinimalEndpoints();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
});

app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

app.Run();

namespace Identity.Api
{
    public partial class Program {}
}
