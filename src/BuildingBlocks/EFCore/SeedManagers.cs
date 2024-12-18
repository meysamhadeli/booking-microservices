using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

public class SeedManager(
IServiceProvider serviceProvider
)
    : ISeedManager
{
    public async Task ExecuteAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedManager>>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var dataSeeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        if (env.IsEnvironment("test"))
        {
            foreach (var seeder in dataSeeders.Where(x => x is ITestDataSeeder))
            {
                logger.LogInformation("Test Seed {SeederName} is started.", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                logger.LogInformation("Test Seed {SeederName} is completed.", seeder.GetType().Name);
            }
        }
        else
        {
            foreach (var seeder in dataSeeders.Where(x => x is not ITestDataSeeder))
            {
                logger.LogInformation("Seed {SeederName} is started.", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                logger.LogInformation("Seed {SeederName} is completed.", seeder.GetType().Name);
            }
        }
    }
}
