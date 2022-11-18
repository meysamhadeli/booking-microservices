using AutoBogus;
using BuildingBlocks.IdsGenerator;
using Flight.Seats.Enums;
using Flight.Seats.Features.CreateSeat.Commands.V1;

namespace Integration.Test.Fakes;

public class FakeCreateSeatCommand : AutoFaker<CreateSeatCommand>
{
    public FakeCreateSeatCommand(long flightId)
    {
        RuleFor(r => r.Id, _ => SnowFlakIdGenerator.NewId());
        RuleFor(r => r.FlightId, _ => flightId);
        RuleFor(r => r.Class, _ => SeatClass.Economy);
        RuleFor(r => r.Type, _ => SeatType.Middle);
    }
}
