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

namespace Flight.Flights.Features.DeleteFlight.Reads;

public class DeleteFlightMongoCommandHandler : ICommandHandler<DeleteFlightMongoCommand>
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

    public async Task<Unit> Handle(DeleteFlightMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var flightReadModel = _mapper.Map<FlightReadModel>(command);

        var flight = await _flightReadDbContext.Flight.AsQueryable()
            .FirstOrDefaultAsync(x => x.FlightId == flightReadModel.FlightId, cancellationToken);

        if (flight is null)
            throw new FlightNotFountException();

        await _flightReadDbContext.Flight.UpdateOneAsync(
            x => x.FlightId == flightReadModel.FlightId,
            Builders<FlightReadModel>.Update
                .Set(x => x.IsDeleted, flightReadModel.IsDeleted),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
