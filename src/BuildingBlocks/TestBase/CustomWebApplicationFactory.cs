using BuildingBlocks.MassTransit;
using BuildingBlocks.Web;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BuildingBlocks.TestBase;

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    public Action<IServiceCollection> TestRegistrationServices { set; get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("test");
        builder.ConfigureServices(services =>
        {
            TestRegistrationServices?.Invoke(services);
            services.ReplaceSingleton(AddHttpContextAccessorMock);
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
