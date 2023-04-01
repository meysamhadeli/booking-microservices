namespace Flight.Seats.Features.ReservingSeat.Commands.V1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CQRS;
using BuildingBlocks.Core.Event;
using Data;
using Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record ReserveSeat(Guid FlightId, string SeatNumber) : ICommand<ReserveSeatResult>, IInternalCommand;

public record ReserveSeatResult(Guid Id);

internal class ReserveSeatValidator : AbstractValidator<ReserveSeat>
{
    public ReserveSeatValidator()
    {
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId must not be empty");
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber must not be empty");
    }
}

internal class ReserveSeatCommandHandler : IRequestHandler<ReserveSeat, ReserveSeatResult>
{
    private readonly FlightDbContext _flightDbContext;

    public ReserveSeatCommandHandler(FlightDbContext flightDbContext)
    {
        _flightDbContext = flightDbContext;
    }

    public async Task<ReserveSeatResult> Handle(ReserveSeat command, CancellationToken cancellationToken)
    {
        Guard.Against.Null(command, nameof(command));

        var seat = await _flightDbContext.Seats.SingleOrDefaultAsync(x => x.SeatNumber == command.SeatNumber && x.FlightId == command.FlightId, cancellationToken);

        if (seat is null)
        {
            throw new SeatNumberIncorrectException();
        }

        var reserveSeat = await seat.ReserveSeat(seat);

        var updatedSeat = (_flightDbContext.Seats.Update(reserveSeat))?.Entity;

        return new ReserveSeatResult(updatedSeat.Id);
    }
}
