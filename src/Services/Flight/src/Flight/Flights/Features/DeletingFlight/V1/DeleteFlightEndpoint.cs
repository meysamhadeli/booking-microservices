namespace Flight.Flights.Features.DeletingFlight.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class DeleteFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/flight/{{id}}", DeleteFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("DeleteFlight")
            .WithMetadata(new SwaggerOperationAttribute("Delete Flight", "Delete Flight"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status204NoContent,
                    "Flight Deleted",
                    typeof(FlightDto)))
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

        return builder;
    }

    private async Task<IResult> DeleteFlight(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteFlight(id), cancellationToken);

        return Results.NoContent();
    }
}
