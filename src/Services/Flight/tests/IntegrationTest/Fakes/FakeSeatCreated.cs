using Flight.Seats.Features.CreateSeat;
using Flight.Seats.Models;

namespace Integration.Test.Fakes;

public static class FakeSeatCreated
{
    public static global::Flight.Seats.Models.Seat Generate(CreateSeatCommand command)
    {
        return global::Flight.Seats.Models.Seat.Create(command.Id, command.SeatNumber, command.Type, command.Class, command.FlightId);
    }
}

