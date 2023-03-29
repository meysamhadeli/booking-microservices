using AutoBogus;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

using global::Flight.Aircrafts.Features.CreatingAircraft.V1;

public class FakeCreateAircraftCommand : AutoFaker<CreateAircraft>
{
    public FakeCreateAircraftCommand()
    {
        RuleFor(r => r.Id, _ => SnowflakeIdGenerator.NewId());
    }
}
