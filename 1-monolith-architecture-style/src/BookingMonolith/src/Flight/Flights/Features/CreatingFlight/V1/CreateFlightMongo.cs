using Ardalis.GuardClauses;
using BookingMonolith.Flight.Data;
using BookingMonolith.Flight.Flights.Exceptions;
using BookingMonolith.Flight.Flights.Models;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BookingMonolith.Flight.Flights.Features.CreatingFlight.V1;

public record CreateFlightMongo(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
                                Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
                                Enums.FlightStatus Status, decimal Price, bool IsDeleted = false) : InternalCommand;

internal class CreateFlightMongoHandler : ICommandHandler<CreateFlightMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateFlightMongoHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateFlightMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flightReadModel = _mapper.Map<FlightReadModel>(request);

        var flight = await _flightReadDbContext.Flight.AsQueryable()
            .FirstOrDefaultAsync(x => x.FlightId == flightReadModel.FlightId && !x.IsDeleted, cancellationToken);

        if (flight is not null)
        {
            throw new FlightAlreadyExistException();
        }

        await _flightReadDbContext.Flight.InsertOneAsync(flightReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
