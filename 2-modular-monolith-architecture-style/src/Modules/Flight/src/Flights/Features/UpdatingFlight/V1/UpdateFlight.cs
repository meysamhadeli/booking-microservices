namespace Flight.Flights.Features.UpdatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Aircrafts.ValueObjects;
using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Data;
using Duende.IdentityServer.EntityFramework.Entities;
using Exceptions;
using Flight.Airports.ValueObjects;
using Flight.Flights.Features.CreatingFlight.V1;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ValueObjects;

public record UpdateFlight(Guid Id, string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, bool IsDeleted, decimal Price) : ICommand<UpdateFlightResult>, IInternalCommand,
    IInvalidateCacheRequest
{
    public string CacheKey => "GetAvailableFlights";
}

public record UpdateFlightResult(Guid Id);

public record FlightUpdatedDomainEvent(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes,
    DateTime FlightDate, Enums.FlightStatus Status, decimal Price, bool IsDeleted) : IDomainEvent;

public record UpdateFlightRequestDto(Guid Id, string FlightNumber, Guid AircraftId, Guid DepartureAirportId,
    DateTime DepartureDate, DateTime ArriveDate,
    Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate, Enums.FlightStatus Status, decimal Price,
    bool IsDeleted);

public class UpdateFlightEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/flight", async (UpdateFlightRequestDto request,
                IMediator mediator,
                IMapper mapper, CancellationToken cancellationToken) =>
            {
                var command = mapper.Map<UpdateFlight>(request);

                await mediator.Send(command, cancellationToken);

                return Results.NoContent();
            })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("UpdateFlight")
            .WithApiVersionSet(builder.NewApiVersionSet("Flight").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Flight")
            .WithDescription("Update Flight")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}

public class UpdateFlightValidator : AbstractValidator<CreateFlight>
{
    public UpdateFlightValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                          p == Enums.FlightStatus.Flying) ||
                                         p == Enums.FlightStatus.Canceled ||
                                         p == Enums.FlightStatus.Delay ||
                                         p == Enums.FlightStatus.Completed)
            .WithMessage("Status must be Flying, Delay, Canceled or Completed");

        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("AircraftId must be not empty");
        RuleFor(x => x.DepartureAirportId).NotEmpty().WithMessage("DepartureAirportId must be not empty");
        RuleFor(x => x.ArriveAirportId).NotEmpty().WithMessage("ArriveAirportId must be not empty");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
        RuleFor(x => x.FlightDate).NotEmpty().WithMessage("FlightDate must be not empty");
    }
}

internal class UpdateFlightHandler : ICommandHandler<UpdateFlight, UpdateFlightResult>
{
    private readonly FlightDbContext _flightDbContext;

    public UpdateFlightHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<UpdateFlightResult> Handle(UpdateFlight request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flight = await _flightDbContext.Flights.SingleOrDefaultAsync(x => x.Id == request.Id,
            cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }


        flight.Update(FlightId.Of(request.Id), FlightNumber.Of(request.FlightNumber), AircraftId.Of(request.AircraftId), AirportId.Of(request.DepartureAirportId),
            DepartureDate.Of(request.DepartureDate),
            ArriveDate.Of(request.ArriveDate), AirportId.Of(request.ArriveAirportId), DurationMinutes.Of(request.DurationMinutes), FlightDate.Of(request.FlightDate), request.Status,
            Price.Of(request.Price), request.IsDeleted);

        var updateFlight = _flightDbContext.Flights.Update(flight).Entity;

        return new UpdateFlightResult(updateFlight.Id);
    }
}
