namespace Flight.Flights.Features.DeletingFlight.V1;

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

public record DeleteFlightMongo(Guid Id, string FlightNumber, Guid AircraftId, DateTime DepartureDate,
    Guid DepartureAirportId, DateTime ArriveDate, Guid ArriveAirportId, decimal DurationMinutes, DateTime FlightDate,
    Enums.FlightStatus Status, decimal Price, bool IsDeleted) : InternalCommand;

internal class DeleteFlightMongoCommandHandler : ICommandHandler<DeleteFlightMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public DeleteFlightMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(DeleteFlightMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var flightReadModel = _mapper.Map<FlightReadModel>(request);

        var flight = await _flightReadDbContext.Flight.AsQueryable()
            .FirstOrDefaultAsync(x => x.FlightId == flightReadModel.FlightId && !x.IsDeleted, cancellationToken);

        if (flight is null)
        {
            throw new FlightNotFountException();
        }

        await _flightReadDbContext.Flight.UpdateOneAsync(
            x => x.FlightId == flightReadModel.FlightId,
            Builders<FlightReadModel>.Update
                .Set(x => x.IsDeleted, flightReadModel.IsDeleted),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
