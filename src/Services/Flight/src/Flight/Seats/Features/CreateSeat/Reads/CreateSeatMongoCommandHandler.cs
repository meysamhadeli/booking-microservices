using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Seats.Exceptions;
using Flight.Seats.Models.Reads;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Flight.Seats.Features.CreateSeat.Reads;

public class CreateSeatMongoCommandHandler : ICommandHandler<CreateSeatMongoCommand>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateSeatMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateSeatMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seatReadModel = _mapper.Map<SeatReadModel>(command);

        var seat = await _flightReadDbContext.Seat.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == seatReadModel.Id, cancellationToken);

        if (seat is not null)
            throw new SeatAlreadyExistException();

        await _flightReadDbContext.Seat.InsertOneAsync(seatReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
