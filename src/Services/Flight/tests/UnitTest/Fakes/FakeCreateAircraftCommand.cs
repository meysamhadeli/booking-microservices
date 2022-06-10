using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Features.CreateAircraft;

namespace Unit.Test.Fakes;

public class FakeCreateAircraftCommand : AutoFaker<CreateAircraftCommand>
{
    public FakeCreateAircraftCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
