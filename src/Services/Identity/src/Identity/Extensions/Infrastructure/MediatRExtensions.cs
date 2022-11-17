using BuildingBlocks.Logging;
using BuildingBlocks.Validation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Extensions.Infrastructure;

public static class MediatRExtensions
{
    public static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(typeof(IdentityRoot).Assembly);
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        return services;
    }
}
