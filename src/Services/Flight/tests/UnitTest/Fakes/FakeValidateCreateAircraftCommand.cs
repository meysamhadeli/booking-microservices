using AutoBogus;
using Flight.Aircrafts.Features.CreateAircraft;

namespace Unit.Test.Fakes;

public class FakeValidateCreateAircraftCommand : AutoFaker<CreateAircraftCommand>
{
    public FakeValidateCreateAircraftCommand()
    {
        RuleFor(r => r.ManufacturingYear, _ => 0);
        RuleFor(r => r.Name, _ => null);
        RuleFor(r => r.Model, _ => null);
    }
}
