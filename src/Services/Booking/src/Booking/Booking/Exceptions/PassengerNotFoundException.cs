using BuildingBlocks.Exception;

namespace Booking.Booking.Exceptions;

public class PassengerNotFoundException: NotFoundException
{
    public PassengerNotFoundException() : base("Flight doesn't exist! ")
    {
    }
}