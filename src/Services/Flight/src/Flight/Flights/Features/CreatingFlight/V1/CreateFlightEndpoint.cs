namespace Flight.Flights.Features.CreatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using Flight.Flights.Dtos;
using Hellang.Middleware.ProblemDetails;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateFlightRequestDto(string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price);

public class CreateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight", CreateFlight)
            .RequireAuthorization()
            .WithTags("Flight")
            .WithName("CreateFlight")
            .WithMetadata(new SwaggerOperationAttribute("Create Flight", "Create Flight"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .WithMetadata(
                new SwaggerResponseAttribute(
                    StatusCodes.Status201Created,
                    "Flight Created",
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

    private async Task<IResult> CreateFlight(CreateFlightRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateFlight>(request);

        var result = await mediator.Send(command, cancellationToken);

        return Results.CreatedAtRoute("GetFlightById", new {id = result.Id}, result);
    }
}
