using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Passenger;

namespace Integration.Test.Fakes;

public class FakePassengerResponse : AutoFaker<PassengerResponse>
{
    public FakePassengerResponse()
    {
        RuleFor(r => r.Id, _ => SnowflakeIdGenerator.NewId());
    }
}
