using BuildingBlocks.Exception;

namespace Flight.Flights.Exceptions;

public class FlightAlreadyExistException : ConflictException
{
    public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", code)
    {
    }
}
