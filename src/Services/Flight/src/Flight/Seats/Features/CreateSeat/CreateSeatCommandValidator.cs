using Flight.Airports.Features.CreateAirport;
using Flight.Seats.Models;
using FluentValidation;

namespace Flight.Seats.Features.CreateSeat;

public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
{
    public CreateSeatCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber is required");
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId is required");
        RuleFor(x => x.Class).Must(p => (p.GetType().IsEnum &&
                                          p == SeatClass.FirstClass) ||
                                         p == SeatClass.Business ||
                                         p == SeatClass.Economy)
            .WithMessage("Status must be FirstClass, Business or Economy");
    }
}
