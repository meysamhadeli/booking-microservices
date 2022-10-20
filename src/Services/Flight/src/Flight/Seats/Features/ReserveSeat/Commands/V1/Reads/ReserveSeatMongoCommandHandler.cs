using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using Flight.Data;
using Flight.Seats.Models.Reads;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;

namespace Flight.Seats.Features.ReserveSeat.Commands.V1.Reads;

public class ReserveSeatMongoCommandHandler : ICommandHandler<ReserveSeatMongoCommand>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public ReserveSeatMongoCommandHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(ReserveSeatMongoCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seatReadModel = _mapper.Map<SeatReadModel>(command);

        await _flightReadDbContext.Seat.UpdateOneAsync(
            x => x.SeatId == seatReadModel.SeatId,
            Builders<SeatReadModel>.Update
                .Set(x => x.IsDeleted, seatReadModel.IsDeleted),
            cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
