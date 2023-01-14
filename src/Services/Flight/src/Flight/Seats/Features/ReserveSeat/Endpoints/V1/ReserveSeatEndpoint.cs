using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Flight.Seats.Features.ReserveSeat.Commands.V1;
using Flight.Seats.Features.ReserveSeat.Dtos.V1;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Seats.Features.ReserveSeat.Endpoints.V1;

using Hellang.Middleware.ProblemDetails;

public class ReserveSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/reserve-seat", ReserveSeat)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("ReserveSeat")
            .WithMetadata(new SwaggerOperationAttribute("Reserve Seat", "Reserve Seat"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "ReserveSeat",
                    typeof(SeatResponseDto)))
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

    private async Task<IResult> ReserveSeat(ReserveSeatRequestDto request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<ReserveSeatCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
