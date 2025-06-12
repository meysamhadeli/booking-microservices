using System.Net;
using BuildingBlocks.Exception;

namespace BookingMonolith.Passenger.Exceptions;

public class PassengerNotExist : AppException
{
    public PassengerNotExist() : base("Please register before!", HttpStatusCode.NotFound)
    {
    }
}
