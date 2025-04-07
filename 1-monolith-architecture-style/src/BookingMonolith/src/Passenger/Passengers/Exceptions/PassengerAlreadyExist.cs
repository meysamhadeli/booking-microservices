using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Passengers.Exceptions;

public class PassengerNotExist : BadRequestException
{
    public PassengerNotExist(string code = default) : base("Please register before!")
    {
    }
}
