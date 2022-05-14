using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

public static class ApiVersioningExtensions
{
    public static void AddCustomVersioning(this IServiceCollection services,
        Action<ApiVersioningOptions> configurator = null)
    {
        //https://www.meziantou.net/versioning-an-asp-net-core-api.htm
        //https://exceptionnotfound.net/overview-of-api-versioning-in-asp-net-core-3-0/
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

            // // Defines how an API version is read from the current HTTP request
            options.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"),
                new UrlSegmentApiVersionReader());

            configurator?.Invoke(options);
        });
    }
}
