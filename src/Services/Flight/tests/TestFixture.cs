using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Flight.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Respawn;
using Xunit;

namespace Integration.Test;

[CollectionDefinition(nameof(TestFixture))]
public class SliceFixtureCollection : ICollectionFixture<TestFixture>
{
}

public class TestFixture : IAsyncLifetime
{
    private readonly Checkpoint _checkpoint;
    private readonly IConfiguration _configuration;
    private readonly WebApplicationFactory<Program> _factory;
    private static IServiceScopeFactory _scopeFactory = null!;

    public TestFixture()
    {
        _factory = new FlightTestApplicationFactory();

        _configuration = _factory.Services.GetRequiredService<IConfiguration>();
        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        _checkpoint = new Checkpoint();
    }

    class FlightTestApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.ConfigureAppConfiguration((_, configBuilder) =>
            // {
            //     configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            //     {
            //         {"ConnectionStrings:DefaultConnection", _connectionString}
            //     });
            // });

            builder.ConfigureServices(services =>
            {
                services.AddLogging();
                var httpContextAccessorService = services.FirstOrDefault(d =>
                    d.ServiceType == typeof(IHttpContextAccessor));
                services.Remove(httpContextAccessorService);
                services.AddSingleton(_ => Mock.Of<IHttpContextAccessor>());

                services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                    w.EnvironmentName == "Flight.IntegrationTest" &&
                    w.ApplicationName == "Flight"));

                // services.AddMassTransitTestHarness();
                //
                // // MassTransit Harness Setup -- Do Not Delete Comment
                // services.AddMassTransitInMemoryTestHarness(cfg =>
                // {
                //     // Consumer Registration -- Do Not Delete Comment
                //     // cfg.AddConsumer<AddToBook>();
                //     // cfg.AddConsumerTestHarness<AddToBook>();
                // });

                EnsureDatabase();
            });
        }

    }


    private static void EnsureDatabase()
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetService<FlightDbContext>();

        context?.Database.Migrate();
    }

    public static TScopedService GetService<TScopedService>()
    {
        var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetService<TScopedService>();
        return service;
    }


    public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetService<ISender>();

        return mediator.Send(request);
    }


    public Task InitializeAsync()
        => _checkpoint.Reset(_configuration.GetConnectionString("FlightConnection"));

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        return Task.CompletedTask;
    }
}
