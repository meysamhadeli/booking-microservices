using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Enums;
using Flight.Seats.Features.CreateSeat;
using Flight.Seats.Models;

namespace Unit.Test.Fakes;

public class FakeCreateSeatCommand : AutoFaker<CreateSeatCommand>
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
