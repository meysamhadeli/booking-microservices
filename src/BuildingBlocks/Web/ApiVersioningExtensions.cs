using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

public static class ApiVersioningExtensions
{
    public static void AddCustomVersioning(
        this IServiceCollection services,
        Action<ApiVersioningOptions>? configurator = null)
    {
        // https://www.meziantou.net/versioning-an-asp-net-core-api.htm
        // https://dotnetthoughts.net/aspnetcore-api-versioning-with-net-6-minimal-apis/
        // https://im5tu.io/article/2022/10/asp.net-core-versioning-minimal-apis/
        // https://www.youtube.com/watch?v=YRJGKyzjFlY
        // https://www.nuget.org/packages/Asp.Versioning.Http

        // Support versioning in minimal apis with (Asp.Versioning.Http) dll
        services.AddApiVersioning(options =>
            {
                // Add the headers "api-supported-versions" and "api-deprecated-versions"
                // This is better for discoverability
                options.ReportApiVersions = true;

                // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously
                // support API versioning. Forcing existing clients to specify an explicit API version for an
                // existing service introduces a breaking change. Conceptually, clients in this situation are
                // bound to some API version of a service, but they don't know what it is and never explicit request it.
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Defines how an API version is read from the current HTTP request
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader());

                configurator?.Invoke(options);
            })
            .AddApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                })

            // Support versioning in mvc with with (Asp.Versioning.Mvc.ApiExplorer) dll
            .AddMvc(); // https://www.nuget.org/packages/Asp.Versioning.Mvc.ApiExplorer
    }
}
