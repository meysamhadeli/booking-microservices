using AutoBogus;
using BuildingBlocks.IdsGenerator;

namespace Unit.Test.Fakes;

using global::Flight.Aircrafts.Features.CreatingAircraft.V1;

public class FakeCreateAircraftCommand : AutoFaker<CreateAircraft>
{
    public FakeCreateAircraftCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
