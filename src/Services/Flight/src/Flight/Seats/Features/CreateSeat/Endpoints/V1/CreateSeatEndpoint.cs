using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Flight.Seats.Features.CreateSeat.Commands.V1;
using Flight.Seats.Features.CreateSeat.Dtos.V1;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

namespace Flight.Seats.Features.CreateSeat.Endpoints.V1;

public class CreateSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/seat", CreateSeat)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("Create Seat")
            .WithMetadata(new SwaggerOperationAttribute("Create Seat", "Create Seat"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .Produces<SeatResponseDto>()
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private async Task<IResult> CreateSeat(CreateSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateSeatCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
