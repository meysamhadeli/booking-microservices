using System.Net;
using Griffin.TestBase;
using Griffin.Web;
using Identity;
using Identity.Api;
using Identity.Extensions.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Contract.Test;

public sealed class IdentityProviderContractFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private RabbitMqContainer? _rabbitMqContainer;
    private IHost? _host;

    public Uri ServerUri { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _postgresContainer = TestContainers.PostgresTestContainer();
        _rabbitMqContainer = TestContainers.RabbitMqTestContainer();

        await _postgresContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        ServerUri = new Uri($"http://127.0.0.1:{GetFreePort()}");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = GetIdentityApiProjectDirectory(),
            EnvironmentName = "test",
        });

        builder.WebHost.UseUrls(ServerUri.ToString());
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AppOptions:Name"] = IdentityContract.AppName,
            ["ConnectionStrings:postgres"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:wolverine"] = _postgresContainer.GetConnectionString(),
            ["ConnectionStrings:identity"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Identity"] = _postgresContainer.GetConnectionString(),
            ["RabbitMqOptions:HostName"] = "127.0.0.1",
            ["RabbitMqOptions:UserName"] = TestContainers.RabbitMqContainerConfiguration.UserName,
            ["RabbitMqOptions:Password"] = TestContainers.RabbitMqContainerConfiguration.Password,
            ["RabbitMqOptions:Port"] = _rabbitMqContainer.GetMappedPublicPort(TestContainers.RabbitMqContainerConfiguration.Port).ToString(),
        });

        builder.AddMinimalEndpoints(assemblies: typeof(IdentityRoot).Assembly);
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

    private static string GetIdentityApiProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(IdentityContract.ProjectDirectory, "..", "src", "Identity.Api"));
    }
}