namespace Flight.Seats.Features.GettingAvailableSeats.V1;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public class GetAvailableSeatsEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-seats/{{id}}", GetAvailableSeats)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("GetAvailableSeats")
            .WithMetadata(new SwaggerOperationAttribute("Get Available Seats", "Get Available Seats"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "GetAvailableSeats",
                    typeof(IEnumerable<SeatDto>)))
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

    private async Task<IResult> GetAvailableSeats(long id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAvailableSeats(id), cancellationToken);

        return Results.Ok(result);
    }
}
