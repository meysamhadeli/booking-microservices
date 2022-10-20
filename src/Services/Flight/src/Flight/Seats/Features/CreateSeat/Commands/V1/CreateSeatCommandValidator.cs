using FluentValidation;

namespace Flight.Seats.Features.CreateSeat.Commands.V1;

public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
{
    public CreateSeatCommandValidator()
    {
        RuleFor(x => x.SeatNumber).NotEmpty().WithMessage("SeatNumber is required");
        RuleFor(x => x.FlightId).NotEmpty().WithMessage("FlightId is required");
        RuleFor(x => x.Class).Must(p => (p.GetType().IsEnum &&
                                          p == Enums.SeatClass.FirstClass) ||
                                         p == Enums.SeatClass.Business ||
                                         p == Enums.SeatClass.Economy)
            .WithMessage("Status must be FirstClass, Business or Economy");
    }
}
