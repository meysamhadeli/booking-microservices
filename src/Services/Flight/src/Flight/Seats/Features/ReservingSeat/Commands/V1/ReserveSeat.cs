namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Flight.Data;
using Flight.Seats.Dtos;
using Flight.Seats.Exceptions;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record ReserveSeat(long FlightId, string SeatNumber) : ICommand<SeatDto>, IInternalCommand;

internal class ReserveSeatValidator : AbstractValidator<ReserveSeat>
{
    public ReserveSeatValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId must not be empty");
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber must not be empty");
    }
}

internal class ReserveSeatCommandHandler : IRequestHandler<ReserveSeat, SeatDto>
{
    private readonly FlightDbContext _flightDbContext;
    private readonly IMapper _mapper;

    public ReserveSeatCommandHandler(IMapper mapper, FlightDbContext flightDbContext)
    {
        _mapper = mapper;
        _flightDbContext = flightDbContext;
    }

    public async Task<SeatDto> Handle(ReserveSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.SeatNumber == command.SeatNumber && x.FlightId == command.FlightId, cancellationToken);

        if (seat is null)
        {
            throw new SeatNumberIncorrectException();
        }

        var reserveSeat = await seat.ReserveSeat(seat);

        var updatedSeat = _flightDbContext.Seats.Update(reserveSeat);

        return _mapper.Map<SeatDto>(updatedSeat.Entity);
    }
}
