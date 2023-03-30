namespace Flight.Flights.Features.CreatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.Web;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Annotations;

public record CreateFlightRequestDto(string FlightNumber, long AircraftId, long DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, long ArriveAirportId,
    decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price);

public record CreateFlightResponseDto(long Id);

public class CreateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/flight", CreateFlight)
            .RequireAuthorization()
            .WithMetadata(new SwaggerOperationAttribute("Create Flight", "Create Flight"))
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces<CreateFlightResponseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .HasApiVersion(1.0);

        return builder;
    }

    private async Task<IResult> CreateFlight(CreateFlightRequestDto request, IMediator mediator, IMapper mapper,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateFlight>(request);

        var result = await mediator.Send(command, cancellationToken);

        var response = new CreateFlightResponseDto(result.Id);

        return Results.CreatedAtRoute("GetFlightById", new {id = result.Id}, response);
    }
}
