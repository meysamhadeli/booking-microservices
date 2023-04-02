namespace Flight.Aircrafts.Features.CreatingAircraft.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Exceptions;
using Models;
using Data;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record CreateAircraftMongo(Guid Id, string Name, string Model, int ManufacturingYear, bool IsDeleted) : InternalCommand;

public class CreateAircraftMongoHandler : ICommandHandler<CreateAircraftMongo>
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
            .FirstOrDefaultAsync(x => x.AircraftId == aircraftReadModel.AircraftId, cancellationToken);

        if (aircraft is not null)
        {
            throw new AircraftAlreadyExistException();
        }

        await _flightReadDbContext.Aircraft.InsertOneAsync(aircraftReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
