namespace Flight.Flights.Features.CreatingFlight.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Exceptions;
using MapsterMapper;
using MediatR;
using Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record CreateFlightMongo(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price, bool IsDeleted) : InternalCommand;

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
