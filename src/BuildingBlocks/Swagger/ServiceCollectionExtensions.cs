using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace BuildingBlocks.Swagger;

public static class ServiceCollectionExtensions
{
    public const string HeaderName = "X-Api-Key";
    public const string HeaderVersion = "api-version";

    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md
    // https://github.com/dotnet/aspnet-api-versioning/tree/88323136a97a59fcee24517a514c1a445530c7e2/examples/AspNetCore/WebApi/MinimalOpenApiExample
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
         // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi
        services.AddEndpointsApiExplorer();

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddOptions<SwaggerOptions>().Bind(configuration.GetSection(nameof(SwaggerOptions)))
            .ValidateDataAnnotations();

        services.AddSwaggerGen(
            options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                foreach (var assembly in assemblies)
                {
                    var xmlFile = XmlCommentsFilePath(assembly);

                    if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);
                }

                options.AddEnumsWithValuesFixFilters();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"},
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                ////https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/467
                // options.OperationFilter<TagByApiExplorerSettingsOperationFilter>();
                // options.OperationFilter<TagBySwaggerOperationFilter>();

                // Enables Swagger annotations (SwaggerOperationAttribute, SwaggerParameterAttribute etc.)
                // options.EnableAnnotations();
            });

        services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);

        static string XmlCommentsFilePath(Assembly assembly)
        {
            var basePath = Path.GetDirectoryName(assembly.Location);
            var fileName = assembly.GetName().Name + ".xml";
            return Path.Combine(basePath, fileName);
        }

        return services;
    }

    public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                var descriptions = app.DescribeApiVersions();

                // build a swagger endpoint for each discovered API version
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });

        return app;
    }
}
