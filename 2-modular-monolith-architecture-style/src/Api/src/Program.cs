using Api.Extensions;
using Booking.Extensions.Infrastructure;
using BuildingBlocks.Web;
using Flight.Extensions.Infrastructure;
using Identity.Extensions.Infrastructure;
using Passenger.Extensions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddMinimalEndpoints(assemblies: AppDomain.CurrentDomain.GetAssemblies());

builder.AddSharedInfrastructure();

builder.AddFlightModules();
builder.AddIdentityModules();
builder.AddPassengerModules();
builder.AddBookingModules();


var app = builder.Build();

// ref: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-7.0#routing-basics
app.UseAuthentication();
app.UseAuthorization();
app.MapMinimalEndpoints();
app.UserSharedInfrastructure();

app.UseFlightModules();
app.UseIdentityModules();
app.UsePassengerModules();
app.UseBookingModules();


app.Run();

namespace Api
{
    public partial class Program
    {
    }
}
