using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EFCore;

public class SeedManager(
    ILogger<SeedManager> logger,
    IWebHostEnvironment env,
    IServiceProvider serviceProvider
) : ISeedManager
{
    public async Task ExecuteSeedAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dataSeeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        if (!env.IsEnvironment("test"))
        {
            foreach (var seeder in dataSeeders.Where(x => x is not ITestDataSeeder))
            {
                logger.LogInformation("Seed {SeederName} is started.", seeder.GetType().Name);
                await seeder.SeedAllAsync();
                logger.LogInformation("Seed {SeederName} is completed.", seeder.GetType().Name);
            }
        }
    }

    public async Task ExecuteTestSeedAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dataSeeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var seeder in dataSeeders.Where(x => x is ITestDataSeeder))
        {
            logger.LogInformation("Seed {SeederName} is started.", seeder.GetType().Name);
            await seeder.SeedAllAsync();
            logger.LogInformation("Seed {SeederName} is completed.", seeder.GetType().Name);
        }
    }
}
