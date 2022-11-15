using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web;

public interface IMinimalEndpoint
{
    IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder);
}
