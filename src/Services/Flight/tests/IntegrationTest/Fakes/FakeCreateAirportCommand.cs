using AutoBogus;
using BuildingBlocks.IdsGenerator;

namespace Integration.Test.Fakes;

using global::Flight.Airports.Features.CreatingAirport.V1;

public class FakeCreateAirportCommand : AutoFaker<CreateAirport>
{
    public FakeCreateAirportCommand()
    {
        RuleFor(r => r.Id, _ => SnowflakeIdGenerator.NewId());
    }
}
