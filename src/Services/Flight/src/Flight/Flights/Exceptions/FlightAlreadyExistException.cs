using BuildingBlocks.Exception;

namespace Flight.Flights.Exceptions;

public class FlightAlreadyExistException : ConflictException
{
    public FlightAlreadyExistException(string code = default) : base("Flight already exist!", code)
    {
    }
}
