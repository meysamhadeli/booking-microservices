using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Flight.Flights.Features.GetFlightById.Queries.V1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.GetFlightById.Endpoints.V1;

public class GetFlightByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{EndpointConfig.BaseApiPath}/flight/{{id}}", GetById)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("Get Flight By Id")
            .WithMetadata(new SwaggerOperationAttribute("Get Flight By Id", "Get Flight By Id"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<FlightResponseDto>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> GetById(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetFlightByIdQuery(id), cancellationToken);

        return Results.Ok(result);
    }
}
