using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flight.Seats.Features.ReserveSeat;

public class ReserveSeatCommandHandler : IRequestHandler<ReserveSeatCommand, SeatResponseDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public ReserveSeatCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<SeatResponseDto> Handle(ReserveSeatCommand command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.SeatNumber == command.SeatNumber && x.FlightId == command.FlightId
            && !x.IsDeleted, cancellationToken);

        if (seat is null)
            throw new SeatNumberIncorrectException();

        var reserveSeat = await seat.ReserveSeat(seat);

        var updatedSeat = _flightDbContext.Seats.Update(reserveSeat);

        return _mapper.Map<SeatResponseDto>(updatedSeat.Entity);
    }
}
