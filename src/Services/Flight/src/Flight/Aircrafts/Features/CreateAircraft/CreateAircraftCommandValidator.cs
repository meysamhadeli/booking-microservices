using FluentValidation;

namespace Flight.Aircrafts.Features.CreateAircraft;

public class CreateAircraftCommandValidator : AbstractValidator<CreateAircraftCommand>
{
    public CreateAircraftCommandValidator()
    {
        CascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Model).NotEmpty().WithMessage("Model is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.ManufacturingYear).NotEmpty().WithMessage("ManufacturingYear is required");
    }
}
