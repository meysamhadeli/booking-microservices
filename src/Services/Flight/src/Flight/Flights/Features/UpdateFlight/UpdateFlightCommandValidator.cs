using Flight.Flights.Features.CreateFlight;
using Flight.Flights.Models;
using FluentValidation;

namespace Flight.Flights.Features.UpdateFlight;

public class UpdateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public UpdateFlightCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Status).Must(p => (p.GetType().IsEnum &&
                                         p == FlightStatus.Flying) ||
                                         p == FlightStatus.Canceled ||
                                         p == FlightStatus.Delay ||
                                         p == FlightStatus.Completed)
            .WithMessage("Status must be Flying, Delay, Canceled or Completed");

        RuleFor(x => x.AircraftId).NotEmpty().WithMessage("AircraftId must be not empty");
        RuleFor(x => x.DepartureAirportId).NotEmpty().WithMessage("DepartureAirportId must be not empty");
        RuleFor(x => x.ArriveAirportId).NotEmpty().WithMessage("ArriveAirportId must be not empty");
        RuleFor(x => x.DurationMinutes).GreaterThan(0).WithMessage("DurationMinutes must be greater than 0");
        RuleFor(x => x.FlightDate).NotEmpty().WithMessage("FlightDate must be not empty");
    }
}
