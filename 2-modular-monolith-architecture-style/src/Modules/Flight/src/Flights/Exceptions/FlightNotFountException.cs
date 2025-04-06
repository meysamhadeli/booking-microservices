using BuildingBlocks.Exception;

namespace Flight.Flights.Exceptions;

public class FlightNotFountException : NotFoundException
{
    public FlightNotFountException() : base("Flight not found!")
    {
    }
}
