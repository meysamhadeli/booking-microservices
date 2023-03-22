namespace Flight.Flights.Features.UpdatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Dtos;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record UpdateFlightRequestDto(long Id, string FlightNumber, long AircraftId, long DepartureAirportId, DateTime DepartureDate, DateTime ArriveDate,
    long ArriveAirportId, decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted);

public class UpdateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/flight", UpdateFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("UpdateFlight")
            .WithMetadata(new SwaggerOperationAttribute("Update Flight", "Update Flight"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<FlightDto>()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status204NoContent,
                    "Flight Updated"))
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

    private async Task<IResult> UpdateFlight(UpdateFlightRequestDto request, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var command = mapper.Map<UpdateFlight>(request);

        await mediator.Send(command, cancellationToken);

        return Results.NoContent();
    }
}
