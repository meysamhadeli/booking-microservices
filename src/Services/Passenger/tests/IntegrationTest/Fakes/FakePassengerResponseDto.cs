using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Passenger;

public class FakePassengerResponse : AutoFaker<PassengerResponse>
{
    public FakePassengerResponse()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
    }
}
