using AutoBogus;
using Flight.Airports.Features.CreateAirport;

namespace Unit.Test.Fakes;

public class FakeValidateCreateAirportCommand : AutoFaker<CreateAirportCommand>
{
    public FakeValidateCreateAirportCommand()
    {
        RuleFor(r => r.Code, _ => null);
        RuleFor(r => r.Name, _ => null);
        RuleFor(r => r.Address, _ => null);
    }
}
