using Microsoft.AspNetCore.Routing;

namespace Woo.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}