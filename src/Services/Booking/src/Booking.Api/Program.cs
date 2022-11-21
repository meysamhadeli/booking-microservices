using Booking.Extensions.Infrastructure;
using BuildingBlocks.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddMinimalEndpoints();
builder.AddInfrastructure();

var app = builder.Build();

app.MapMinimalEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseInfrastructure();

app.Run();

namespace Booking.Api
{
    public partial class Program
    {
    }
}
