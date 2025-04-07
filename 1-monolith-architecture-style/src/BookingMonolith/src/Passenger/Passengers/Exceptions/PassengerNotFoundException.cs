using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Passengers.Exceptions;

public class PassengerNotFoundException : NotFoundException
{
    public PassengerNotFoundException(string code = default) : base("Passenger not found!")
    {
    }
}
