using AutoBogus;
using Flight.Seats.Enums;
using Flight.Seats.Features.CreateSeat;
using Flight.Seats.Models;

namespace Unit.Test.Fakes;

public class FakeValidateCreateSeatCommand : AutoFaker<CreateSeatCommand>
{
    public FakeValidateCreateSeatCommand()
    {
        RuleFor(r => r.SeatNumber, _ => null);
        RuleFor(r => r.FlightId, _ => 0);
        RuleFor(r => r.Class, _ => (SeatClass)10);
    }
}
