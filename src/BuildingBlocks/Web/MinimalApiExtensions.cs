using System.Reflection;
using BuildingBlocks.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BuildingBlocks.Web;

public static class MinimalApiExtensions
{

    public static IServiceCollection AddMinimalEndpoints(
        this WebApplicationBuilder applicationBuilder,
        ServiceLifetime lifetime = ServiceLifetime.Scoped,
        params Assembly[] assemblies)
    {

        var scanAssemblies = assemblies.Any()
            ? assemblies
            : TypeProvider.GetReferencedAssemblies(Assembly.GetCallingAssembly())
                .Concat(TypeProvider.GetApplicationPartAssemblies(Assembly.GetCallingAssembly()))
                .Distinct()
                .ToArray();

        applicationBuilder.Services.Scan(scan => scan
            .FromAssemblies(scanAssemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IMinimalEndpoint)))
            .UsingRegistrationStrategy(RegistrationStrategy.Append)
            .As<IMinimalEndpoint>()
            .WithLifetime(lifetime));

        return applicationBuilder.Services;
    }

    /// <summary>
    /// Map Minimal Endpoints
    /// </summary>
    /// <name>builder.</name>
    /// <returns>IEndpointRouteBuilder.</returns>
    public static IEndpointRouteBuilder MapMinimalEndpoints(this IEndpointRouteBuilder builder)
    {
        var scope = builder.ServiceProvider.CreateScope();

        var endpoints = scope.ServiceProvider.GetServices<IMinimalEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return builder;
    }
}
