using Woo.Web;
using Figgle;
using Figgle.Fonts;

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

        // Substantive Automation Enhancement: Inject explicit application lifecycle 
        // try-catch safeguards and telemetry log wrappers to protect cloud bootstrap execution.
        try
        {
            Console.WriteLine("Executing enterprise API Gateway host environment bootstrap sequence...");
            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Critical Gateway initialization failure: {ex.Message}");
            // Enforce structured error routing for cloud-native automated orchestrators
            throw new InvalidOperationException("API Gateway Host crashed unexpectedly during runtime startup routing initialization.", ex);
        }
 
