using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Enums;

namespace Integration.Test.Fakes;

using global::Flight.Seats.Features.CreatingSeat.V1;

public class FakeCreateSeatCommand : AutoFaker<CreateSeat>
{
    public FakeCreateSeatCommand(long flightId)
    {
        RuleFor(r => r.Id, _ => SnowflakeIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => flightId);
        RuleFor(r => r.Class, _ => SeatClass.Economy);
        RuleFor(r => r.Type, _ => SeatType.Middle);
    }
}
