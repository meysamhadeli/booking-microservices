using Asp.Versioning.Builder;

namespace Woo.Web;

public class EndpointConfig
{
    public const string BaseApiPath = "api/v{version:apiVersion}";
    public static ApiVersionSet VersionSet { get; private set; } = default!;
}