namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

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
using ValueObjects;

public record CreateAircraftMongo(Guid Id, string Name, string Model, int ManufacturingYear, bool IsDeleted = false) : InternalCommand;

internal class CreateAircraftMongoHandler : ICommandHandler<CreateAircraftMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateAircraftMongoHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateAircraftMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var aircraftReadModel = _mapper.Map<AircraftReadModel>(request);

        var aircraft = await _flightReadDbContext.Aircraft.AsQueryable()
            .FirstOrDefaultAsync(x => x.AircraftId == aircraftReadModel.AircraftId &&
                                      !x.IsDeleted, cancellationToken);

        if (aircraft is not null)
        {
            throw new AircraftAlreadyExistException();
        }

        await _flightReadDbContext.Aircraft.InsertOneAsync(aircraftReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
