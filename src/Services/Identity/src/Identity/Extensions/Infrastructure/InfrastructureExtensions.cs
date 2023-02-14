using System;
using System.Threading.RateLimiting;
using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.IdsGenerator;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.OpenTelemetry;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.Swagger;
using BuildingBlocks.Web;
using Figgle;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Identity.Data;
using Identity.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Serilog;

namespace Identity.Extensions.Infrastructure;

using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, EventMapper>();
        builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(FiggleFonts.Standard.Render(appOptions.Name));

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true, PermitLimit = 10, QueueLimit = 0, Window = TimeSpan.FromMinutes(1)
                    }));
        });

        builder.Services.AddControllers();
        builder.Services.AddPersistMessageProcessor();
        builder.Services.AddCustomDbContext<IdentityContext>();
        builder.Services.AddScoped<IDataSeeder, IdentityDataSeeder>();
        builder.AddCustomSerilog(env);
        builder.Services.AddCustomSwagger(configuration, typeof(IdentityRoot).Assembly);
        builder.Services.AddCustomVersioning();
        builder.Services.AddCustomMediatR();
        builder.Services.AddValidatorsFromAssembly(typeof(IdentityRoot).Assembly);
        builder.Services.AddCustomProblemDetails();
        builder.Services.AddCustomMapster(typeof(IdentityRoot).Assembly);
        builder.Services.AddCustomHealthCheck();

        builder.Services.AddCustomMassTransit(env, typeof(IdentityRoot).Assembly);
        builder.Services.AddCustomOpenTelemetry();

        SnowFlakIdGenerator.Configure(4);

        builder.Services.AddCustomIdentityServer(env);

        //ref: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-2.2
        //ref: https://medium.com/@christopherlenard/identity-server-and-nginx-ingress-controller-in-kubernetes-7146c22a2466
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return builder;
    }


    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var env = app.Environment;
        var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

        //ref: https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-2.2
        //ref: https://medium.com/@christopherlenard/identity-server-and-nginx-ingress-controller-in-kubernetes-7146c22a2466
        app.UseForwardedHeaders();

        app.UseProblemDetails();
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = LogEnrichHelper.EnrichFromRequest;
        });
        app.UseMigration<IdentityContext>(env);
        app.UseCorrelationId();
        app.UseHttpMetrics();
        app.UseProblemDetails();
        app.UseCustomHealthCheck();
        app.UseIdentityServer();
        app.MapMetrics();

        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        app.Use((httpContext, next) =>
        {
            httpContext.Request.Scheme = "https";
            return next();
        });

        if (env.IsDevelopment())
        {
            app.UseCustomSwagger();
        }

        return app;
    }
}
