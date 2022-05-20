using FluentValidation;

namespace Flight.Flights.Features.DeleteFlight;

public class DeleteFlightCommandValidator : AbstractValidator<DeleteFlightCommand>
{
    public DeleteFlightCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id).NotEmpty();
    }
}

