namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Data;
using Flight.Seats.Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;

public record ReserveSeatMongo(Guid Id, string SeatNumber, Enums.SeatType Type,
    Enums.SeatClass Class, Guid FlightId, bool IsDeleted) : InternalCommand;

internal class ReserveSeatMongoHandler : ICommandHandler<ReserveSeatMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public ReserveSeatMongoHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(ReserveSeatMongo command, CancellationToken cancellationToken)
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
