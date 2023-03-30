namespace Flight.Seats.Features.CreatingSeat.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateSeatRequestDto(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId);
public record CreateSeatResponseDto(long Id);

public class CreateSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight/seat", CreateSeat)
            .RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Create Seat", "Create Seat"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateSeatResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CreateSeat(CreateSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new CreateSeatResponseDto(result.Id);

        return Results.Ok(response);
    }
}
