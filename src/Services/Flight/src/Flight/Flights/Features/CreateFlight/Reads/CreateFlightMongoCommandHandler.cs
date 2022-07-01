using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Exceptions;
using Flight.Flights.Models.Reads;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Flights.Features.CreateFlight.Reads;

public class CreateFlightMongoCommandHandler : ICommandHandler<CreateFlightMongoCommand>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateFlightMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateFlightMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flightReadModel = _mapper.Map<FlightReadModel>(command);

        var flight = await _flightReadDbContext.Flight.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (flight is not null)
            throw new FlightAlreadyExistException();

        await _flightReadDbContext.Flight.InsertOneAsync(flightReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
