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

using Hellang.Middleware.ProblemDetails;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}", DeleteFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("DeleteFlight")
            .WithMetadata(new SwaggerOperationAttribute("Delete Flight", "Delete Flight"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status204NoContent,
                    "Flight Deleted",
                    typeof(FlightResponseDto)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status400BadRequest,
                    "BadRequest",
                    typeof(StatusCodeProblemDetails)))
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status401Unauthorized,
                    "UnAuthorized",
                    typeof(StatusCodeProblemDetails)))
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> DeleteFlight(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteFlightCommand(id), cancellationToken);

        return Results.NoContent();
    }
}
