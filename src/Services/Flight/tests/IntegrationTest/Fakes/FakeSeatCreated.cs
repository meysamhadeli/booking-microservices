using Flight.Seats.Features.CreateSeat;
using Flight.Seats.Models;

namespace Integration.Test.Fakes;

public static class FakeSeatCreated
{
    public static Seat Generate(CreateSeatCommand command)
    {
        return Seat.Create(command.Id, command.SeatNumber, command.Type, command.Class, command.FlightId);
    }
}

