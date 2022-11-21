using BuildingBlocks.Web;
using Passenger.Extensions.Infrastructure;

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
