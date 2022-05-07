using FluentValidation;

namespace Flight.Seats.Features.ReserveSeat;

public class ReserveSeatCommandValidator : AbstractValidator<ReserveSeatCommand>
{
    public ReserveSeatCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId must not be empty");
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber must not be empty");
    }
}
