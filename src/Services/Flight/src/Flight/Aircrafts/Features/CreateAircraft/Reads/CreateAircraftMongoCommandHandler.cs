using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Aircrafts.Exceptions;
using Flight.Aircrafts.Models.Reads;
using Flight.Data;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Aircrafts.Features.CreateAircraft.Reads;

public class CreateAircraftMongoCommandHandler : ICommandHandler<CreateAircraftMongoCommand>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateAircraftMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateAircraftMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var aircraftReadModel = _mapper.Map<AircraftReadModel>(command);

        var aircraft = await _flightReadDbContext.Aircraft.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == aircraftReadModel.Id, cancellationToken);

        if (aircraft is not null)
            throw new AircraftAlreadyExistException();

        await _flightReadDbContext.Aircraft.InsertOneAsync(aircraftReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
