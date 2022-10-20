using FluentValidation;

namespace Flight.Flights.Features.DeleteFlight.Commands.V1;

public class DeleteFlightCommandValidator : AbstractValidator<DeleteFlightCommand>
{
    public DeleteFlightCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id).NotEmpty();
    }
}

