using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Flight.Seats.Features.GetAvailableSeats.Queries.V1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Seats.Features.GetAvailableSeats.Endpoints.V1;

public class GetAvailableSeatsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-seats/{{id}}", GetAvailableSeats)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("GetAvailableSeats")
            .WithMetadata(new SwaggerOperationAttribute("Get Available Seats", "Get Available Seats"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<IEnumerable<SeatResponseDto>>()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> GetAvailableSeats(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableSeatsQuery(id), cancellationToken);

        return Results.Ok(result);
    }
}
