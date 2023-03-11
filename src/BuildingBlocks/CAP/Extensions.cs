using System.Text.Encodings.Web;
using System.Text.Unicode;
using BuildingBlocks.Utils;
using BuildingBlocks.Web;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.CAP;

public static class Extensions
{
    public static IServiceCollection AddCustomCap<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        var rabbitMqOptions = services.GetOptions<RabbitMQOptions>(nameof(RabbitMQOptions));

        services.AddCap(x =>
        {
            x.UseEntityFramework<TDbContext>();
            x.UseRabbitMQ(o =>
            {
                o.HostName = rabbitMqOptions.HostName;
                o.UserName = rabbitMqOptions.UserName;
                o.Password = rabbitMqOptions.Password;
            });
            x.UseDashboard();
            x.FailedRetryCount = 5;
            x.FailedThresholdCallback = failed =>
            {
                var logger = failed.ServiceProvider.GetService<ILogger>();
                logger?.LogError(
                    $@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times,
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
            };
            x.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        });

        // services.AddOpenTelemetryTracing((builder) => builder
        //     .AddAspNetCoreInstrumentation()
        //     .AddCapInstrumentation()
        //     .AddZipkinExporter()
        // );

        services.Scan(s =>
            s.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                .AddClasses(c => c.AssignableTo(typeof(ICapSubscribe)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;
    }
}
