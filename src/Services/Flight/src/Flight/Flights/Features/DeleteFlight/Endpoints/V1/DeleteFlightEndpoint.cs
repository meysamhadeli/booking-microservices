using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Flight.Flights.Features.DeleteFlight.Commands.V1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Flights.Features.DeleteFlight.Endpoints.V1;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}", DeleteFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("Delete Flight")
            .WithMetadata(new SwaggerOperationAttribute("Delete Flight", "Delete Flight"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<FlightResponseDto>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> DeleteFlight(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteFlightCommand(id), cancellationToken);

        return Results.Ok(result);
    }
}
