using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Woo.TestBase;
using Woo.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using PactNet.Verifier;
using Passenger;
using Passenger.Api;
using Passenger.Data;
using Passenger.Extensions.Infrastructure;
using Passenger.Passengers.Models;
using Xunit;

namespace Contract.Test;

using Duende.IdentityServer.EntityFramework.Entities;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

public sealed class PassengerProviderContractFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;
    private MongoDbContainer? _mongoContainer;
    private IHost? _host;

    public Uri ServerUri { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _postgresContainer = TestContainers.PostgresTestContainer();
        _mongoContainer = TestContainers.MongoTestContainer();

        await _postgresContainer.StartAsync();
        await _mongoContainer.StartAsync();

        ServerUri = new Uri($"http://127.0.0.1:{GetFreePort()}");

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = GetPassengerApiProjectDirectory(),
            EnvironmentName = "test",
        });

        builder.WebHost.UseUrls(ServerUri.ToString());
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["AppOptions:Name"] = "Passenger.Api.Contract",
            ["Jwt:Authority"] = "http://localhost",
            ["Jwt:Audience"] = "flight-api",
            ["PostgresOptions:ConnectionString"] = _postgresContainer.GetConnectionString(),
            ["PostgresOptions:ConnectionString:Passenger"] = _postgresContainer.GetConnectionString(),
            ["MongoOptions:ConnectionString"] = _mongoContainer.GetConnectionString(),
            ["MongoOptions:DatabaseName"] = TestContainers.MongoContainerConfiguration.Name,
        });

        builder.AddMinimalEndpoints(assemblies: typeof(PassengerRoot).Assembly);
        builder.AddInfrastructure();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ContractTestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = ContractTestAuthenticationHandler.SchemeName;
                options.DefaultScheme = ContractTestAuthenticationHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, ContractTestAuthenticationHandler>(
                ContractTestAuthenticationHandler.SchemeName,
                _ => { });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(nameof(ApiScope), policy =>
            {
                policy.AuthenticationSchemes.Clear();
                policy.AuthenticationSchemes.Add(ContractTestAuthenticationHandler.SchemeName);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "flight-api");
            });
        });

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddScoped<PassengerProviderStateSeeder>();

        var app = builder.Build();

        app.MapPost("/provider-states", async (ProviderStateRequest request, PassengerProviderStateSeeder seeder, CancellationToken cancellationToken) =>
        {
            await seeder.SeedAsync(request.State, cancellationToken);
            return Results.Ok();
        });

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

        if (_mongoContainer is not null)
        {
            await _mongoContainer.StopAsync();
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

    private static string GetPassengerApiProjectDirectory()
    {
        return Path.GetFullPath(Path.Combine(PassengerContract.ProjectDirectory, "..", "src", "Passenger.Api"));
    }
}

public sealed class PassengerProviderStateSeeder(PassengerReadDbContext passengerReadDbContext)
{
    public async Task SeedAsync(string state, CancellationToken cancellationToken)
    {
        await passengerReadDbContext.Passenger.DeleteManyAsync(_ => true, cancellationToken);

        if (state != PassengerContract.ProviderState)
        {
            return;
        }

        await passengerReadDbContext.Passenger.InsertOneAsync(
            new PassengerReadModel
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                PassengerId = PassengerContract.PassengerId,
                Name = PassengerContract.PassengerName,
                PassportNumber = PassengerContract.PassportNumber,
                PassengerType = Enum.Parse<Passenger.Passengers.Enums.PassengerType>(PassengerContract.PassengerType),
                Age = PassengerContract.Age,
                IsDeleted = false,
            },
            cancellationToken: cancellationToken);
    }
}

public sealed record ProviderStateRequest(string State);

internal sealed class ContractTestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ContractTest";

    public ContractTestAuthenticationHandler(
        Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
        Microsoft.Extensions.Logging.ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "contract-user"),
                new Claim(ClaimTypes.Name, "contract-user"),
                new Claim("scope", "flight-api"),
            },
            SchemeName);

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}