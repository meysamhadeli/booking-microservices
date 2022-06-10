using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Airports.Features.CreateAirport;

namespace Unit.Test.Fakes;

public class FakeCreateAirportCommand : AutoFaker<CreateAirportCommand>
{
    public FakeCreateAirportCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
