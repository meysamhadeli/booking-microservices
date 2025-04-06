namespace Flight.Flights.Exceptions;

using BuildingBlocks.Exception;

public class FlightAlreadyExistException : ConflictException
{
    public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", code)
    {
    }
}
