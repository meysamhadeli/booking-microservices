using System;
using BuildingBlocks.MassTransit;
using BuildingBlocks.Web;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Respawn;

namespace Integration.Test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Checkpoint Checkpoint { get; set; }
    public IConfiguration Configuration => Services.GetRequiredService<IConfiguration>();
    public Action<IServiceCollection>? TestRegistrationServices { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests#set-the-environment
        //https://stackoverflow.com/questions/43927955/should-getenvironmentvariable-work-in-xunit-test/43951218

        //we could read env from our test launch setting or we can set it directly here
        builder.UseEnvironment("test");

        //The test app's builder.ConfigureTestServices callback is executed after the app's Startup.ConfigureServices code is executed.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
            services.ReplaceSingleton(AddHttpContextAccessorMock);
            TestRegistrationServices?.Invoke(services);
            services.AddMassTransitTestHarness(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqOptions = services.GetOptions<RabbitMqOptions>("RabbitMq");
                    var host = rabbitMqOptions.HostName;

                    cfg.Host(host, h =>
                    {
                        h.Username(rabbitMqOptions.UserName);
                        h.Password(rabbitMqOptions.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });

            Checkpoint = new Checkpoint {TablesToIgnore = new[] {"__EFMigrationsHistory"}};

            TestRegistrationServices?.Invoke(services);
        });
    }

    private IHttpContextAccessor AddHttpContextAccessorMock(IServiceProvider serviceProvider)
    {
        var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
        using var scope = serviceProvider.CreateScope();
        httpContextAccessorMock.HttpContext = new DefaultHttpContext {RequestServices = scope.ServiceProvider};

        httpContextAccessorMock.HttpContext.Request.Host = new HostString("localhost", 6012);
        httpContextAccessorMock.HttpContext.Request.Scheme = "http";

        return httpContextAccessorMock;
    }
}
