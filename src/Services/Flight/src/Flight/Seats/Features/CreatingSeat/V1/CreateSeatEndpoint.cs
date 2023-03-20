namespace Flight.Seats.Features.CreatingSeat.V1;

using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Seats.Dtos;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateSeatRequestDto(string SeatNumber, Enums.SeatType Type, Enums.SeatClass Class, long FlightId);

public class CreateSeatEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost($"{EndpointConfig.BaseApiPath}/flight/seat", CreateSeat)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("CreateSeat")
            .WithMetadata(new SwaggerOperationAttribute("Create Seat", "Create Seat"))
            .WithApiVersionSet(endpoints.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "Seat Created",
                    typeof(SeatDto)))
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

    private async Task<IResult> CreateSeat(CreateSeatRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateSeat>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
