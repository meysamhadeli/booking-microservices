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
using Passenger.Extensions.Infrastructure;
using Passenger.GrpcServer.Services;
using Prometheus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.AddMinimalEndpoints();
builder.AddInfrastructure();

var app = builder.Build();

app.MapMinimalEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseInfrastructure();

app.Run();

namespace Passenger.Api
{
    public partial class Program
    {
    }
}
