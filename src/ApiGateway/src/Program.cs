using BuildingBlocks.Web;
using Figgle;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var appOptions = builder.Services.GetOptions<AppOptions>("AppOptions");
Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));


builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("Yarp"));

var app = builder.Build();

app.UseCorrelationId();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapReverseProxy();
});

app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

app.Run();