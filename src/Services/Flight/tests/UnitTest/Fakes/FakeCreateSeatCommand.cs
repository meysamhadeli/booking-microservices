using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Enums;

namespace Unit.Test.Fakes;

using global::Flight.Seats.Features.CreatingSeat.V1;

public class FakeCreateSeatCommand : AutoFaker<CreateSeat>
{
    public FakeCreateSeatCommand()
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => 1);
        RuleFor(r => r.SeatNumber, _ => "F99");
        RuleFor(r => r.Type, _ => SeatType.Window);
        RuleFor(r => r.Class, _ => SeatClass.Economy);
    }
}
