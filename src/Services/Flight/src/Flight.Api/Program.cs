using BuildingBlocks.Web;
using Flight;
using Flight.Extensions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddMinimalEndpoints(assemblies: typeof(FlightRoot).Assembly);
builder.AddInfrastructure();

var app = builder.Build();

// ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#routing-basics
app.MapMinimalEndpoints();
app.UseInfrastructure();

app.Run();

namespace Flight.Api
{
    public partial class Program
    {
    }
}