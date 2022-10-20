using FluentValidation;

namespace Passenger.Passengers.Features.GetPassengerById.Queries.V1;

public class GetPassengerQueryByIdValidator: AbstractValidator<GetPassengerQueryById>
{
    public GetPassengerQueryByIdValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}