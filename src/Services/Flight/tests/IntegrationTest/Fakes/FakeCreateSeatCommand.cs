using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Features.CreateSeat;

namespace Integration.Test.Fakes;

public class FakeCreateSeatCommand : AutoFaker<CreateSeatCommand>
{
    public FakeCreateSeatCommand(long flightId)
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => flightId);
    }
}
