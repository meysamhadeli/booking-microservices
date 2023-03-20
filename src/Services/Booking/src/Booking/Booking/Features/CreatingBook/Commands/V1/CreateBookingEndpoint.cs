namespace Booking.Booking.Features.CreatingBook.Commands.V1;

using BuildingBlocks.Web;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateBookingRequestDto(long PassengerId, long FlightId, string Description);

public class CreateBookingEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/booking", CreateBooking)
            .RequireAuthorization()
            .WithTags("Booking")
            .WithName("CreateBooking")
            .WithMetadata(new SwaggerOperationAttribute("Create Booking", "Create Booking"))
            .WithApiVersionSet(builder.NewApiVersionSet("Booking").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status200OK,
                    "Booking Created",
                    typeof(ulong)))
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

    private async Task<IResult> CreateBooking(CreateBookingRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateBooking>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}
