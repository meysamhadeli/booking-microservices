using FluentValidation;

namespace Passenger.Passengers.Features.GetPassengerById;

public class GetPassengerQueryByIdValidator: AbstractValidator<GetPassengerQueryById>
{
    public GetPassengerQueryByIdValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}