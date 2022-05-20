using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Airports.Features.CreateAirport;

namespace Integration.Test.Fakes;

public class FakeCreateAirportCommand : AutoFaker<CreateAirportCommand>
{
    public FakeCreateAirportCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
