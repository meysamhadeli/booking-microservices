using BuildingBlocks.Exception;

namespace Flight.Flights.Features.CreateFlight.Exceptions;

public class FlightAlreadyExistException : ConflictException
{
    public FlightAlreadyExistException(int? code = default) : base("Flight already exist!", code)
    {
    }
}
