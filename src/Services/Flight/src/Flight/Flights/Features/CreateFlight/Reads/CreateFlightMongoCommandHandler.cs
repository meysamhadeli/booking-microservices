using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Flights.Models.Reads;
using MapsterMapper;
using MediatR;

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

        await _flightReadDbContext.Flight.InsertOneAsync(flightReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
