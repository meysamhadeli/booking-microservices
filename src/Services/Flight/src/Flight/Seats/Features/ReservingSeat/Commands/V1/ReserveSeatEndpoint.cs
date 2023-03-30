namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record ReserveSeatRequestDto(long FlightId, string SeatNumber);
public record ReserveSeatResponseDto(long Id);

public class ReserveSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/reserve-seat", ReserveSeat)
            .RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Reserve Seat", "Reserve Seat"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<ReserveSeatResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> ReserveSeat(ReserveSeatRequestDto request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<V1.ReserveSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new ReserveSeatResponseDto(result.Id);

        return Results.Ok(response);
    }
}
