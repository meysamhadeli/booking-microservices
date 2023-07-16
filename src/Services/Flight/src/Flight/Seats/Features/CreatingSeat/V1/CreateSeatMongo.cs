namespace Flight.Seats.Features.CreatingSeat.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Exceptions;
using Models;
using MapsterMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

public record CreateSeatMongo(Guid Id, string SeatNumber, Enums.SeatType Type,
    Enums.SeatClass Class, Guid FlightId, bool IsDeleted = false) : InternalCommand;

internal class CreateSeatMongoHandler : ICommandHandler<CreateSeatMongo>
{
    private readonly FlightReadDbContext _flightReadDbContext;
    private readonly IMapper _mapper;

    public CreateSeatMongoHandler(
        FlightReadDbContext flightReadDbContext,
        IMapper mapper)
    {
        _flightReadDbContext = flightReadDbContext;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateSeatMongo request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var seatReadModel = _mapper.Map<SeatReadModel>(request);

        var seat = await _flightReadDbContext.Seat.AsQueryable()
            .FirstOrDefaultAsync(x => x.SeatId == seatReadModel.SeatId &&
                                      !x.IsDeleted, cancellationToken);

        if (seat is not null)
        {
            throw new SeatAlreadyExistException();
        }

        await _flightReadDbContext.Seat.InsertOneAsync(seatReadModel, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}
