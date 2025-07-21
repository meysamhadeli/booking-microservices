using Booking;
using Booking.Extensions.Infrastructure;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddMinimalEndpoints(assemblies: typeof(BookingRoot).Assembly);
builder.AddInfrastructure();

var app = builder.Build();

app.MapMinimalEndpoints();
app.UseInfrastructure();

app.Run();

namespace Booking.Api
{
    public partial class Program
    {
    }
}
