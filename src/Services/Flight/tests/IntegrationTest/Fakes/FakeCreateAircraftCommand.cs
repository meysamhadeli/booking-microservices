using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Aircrafts.Features.CreateAircraft;
using Flight.Aircrafts.Features.CreateAircraft.Commands.V1;

namespace Integration.Test.Fakes;

public class FakeCreateAircraftCommand : AutoFaker<CreateAircraftCommand>
{
    public FakeCreateAircraftCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
