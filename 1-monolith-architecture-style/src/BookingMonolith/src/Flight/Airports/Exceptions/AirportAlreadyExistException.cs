using BuildingBlocks.Exception;

namespace BookingMonolith.Flight.Airports.Exceptions;

public class AirportAlreadyExistException : ConflictException
{
    public AirportAlreadyExistException(int? code = default) : base("Airport already exist!", code)
    {
    }
}
