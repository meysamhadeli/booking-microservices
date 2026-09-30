using System.Net;
using Woo.TestBase;
using Woo.Web;
using Flight;
using Flight.Api;
using Flight.Extensions.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.EventStoreDb;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Contract.Test;

public sealed class FlightProviderContractFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private RabbitMqContainer? _rabbitMqContainer;
    private MongoDbContainer? _mongoContainer;
    private EventStoreDbContainer? _eventStoreContainer;
    private IHost? _host;

    public Uri ServerUri { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _postgresContainer = TestContainers.PostgresTestContainer();
        _rabbitMqContainer = TestContainers.RabbitMqTestContainer();
        _mongoContainer = TestContainers.MongoTestContainer();
        _eventStoreContainer = TestContainers.EventStoreTestContainer();

        await _mongoContainer.StartAsync();
        await _postgresContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _eventStoreContainer.StartAsync();

        ServerUri = new Uri($"http://127.0.0.1:{GetFreePort()}");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = GetFlightApiProjectDirectory(),
            EnvironmentName = "test",
        });

        builder.WebHost.UseUrls(ServerUri.ToString());
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AppOptions:Name"] = FlightContract.AppName,
            ["ConnectionStrings:postgres"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:wolverine"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:booking"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:flight"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:identity"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:passenger"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Booking"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Flight"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Identity"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Passenger"] = _postgresContainer.GetConnectionString(),
            ["RabbitMqOptions:HostName"] = "127.0.0.1",
            ["RabbitMqOptions:UserName"] = TestContainers.RabbitMqContainerConfiguration.UserName,
            ["RabbitMqOptions:Password"] = TestContainers.RabbitMqContainerConfiguration.Password,
            ["RabbitMqOptions:Port"] = _rabbitMqContainer.GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration.Port).ToString(),
            ["MongoOptions:ConnectionString"] = _mongoContainer.GetConnectionString(),
            ["MongoOptions:DatabaseName"] = TestContainers.MongoContainerConfiguration.Name,
            ["EventStoreOptions:ConnectionString"] = _eventStoreContainer.GetConnectionString(),
        });

        builder.AddMinimalEndpoints(assemblies: typeof(FlightRoot).Assembly);
        builder.AddInfrastructure();
        builder.Services.RemoveAll<IHostedService>();

        var app = builder.Build();

        app.MapMinimalEndpoints();
        app.UseInfrastructure();

        _host = app;
        await app.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_host is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_host is not null)
        {
            _host.Dispose();
        }

        if (_eventStoreContainer is not null)
        {
            await _eventStoreContainer.StopAsync();
        }

        if (_mongoContainer is not null)
        {
            await _mongoContainer.StopAsync();
        }

        if (_rabbitMqContainer is not null)
        {
            await _rabbitMqContainer.StopAsync();
        }

        if (_postgresContainer is not null)
        {
            await _postgresContainer.StopAsync();
        }
    }

    private static int GetFreePort()
    {
        var listener = new System.Net.Sockets.TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string GetFlightApiProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(FlightContract.ProjectDirectory, "..", "src", "Flight.Api"));
    }
}