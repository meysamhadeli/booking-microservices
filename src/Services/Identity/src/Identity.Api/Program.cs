using BuildingBlocks.Web;
using Identity.Configurations;
using Identity.Extensions.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    // Service provider validation
    // ref: https://andrewlock.net/new-in-asp-net-core-3-service-provider-validation/
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging() || context.HostingEnvironment.IsEnvironment("tests");
    options.ValidateOnBuild = true;
});

builder.AddMinimalEndpoints(assemblies:typeof(IdentityRoot).Assembly);
builder.AddInfrastructure();

var app = builder.Build();

app.MapMinimalEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.UseInfrastructure();

app.Run();

namespace Identity.Api
{
    public partial class Program { }
}
